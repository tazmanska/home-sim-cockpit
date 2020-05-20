/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-13
 * Godzina: 20:54
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;

namespace RS232HCDevices.Steppers
{
    /// <summary>
    /// Description of StepperMotor.
    /// </summary>
    class StepperMotor
    {
        public static StepperMotor Load(XmlNode xml)
        {
            if (xml == null)
            {
                return null;
            }
            StepperMotor result = new StepperMotor();
            result.Id = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            //            result.Enabled = bool.Parse(xml.Attributes["enabled"].Value);
            result.StepsFor360 = int.Parse(xml.Attributes["stepsFor360"].Value);
            result.MinStepInterval = byte.Parse(xml.Attributes["minStepInterval"].Value);
            result.LastPosition = double.Parse(xml.Attributes["lastPosition"].Value.Replace(",", "."), NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            result.LastStepDirection = (StepDirection)Enum.Parse(typeof(StepDirection), xml.Attributes["lastStepDirection"].Value);
            result.LastStepType = (StepType)Enum.Parse(typeof(StepType), xml.Attributes["lastStepType"].Value);
            result.LastStepIndex = int.Parse(xml.Attributes["lastStepIndex"].Value);
            result.KeepTourque = bool.Parse(xml.Attributes["keepTourque"].Value);
            result.HasZeroSensor = bool.Parse(xml.Attributes["hasZeroSensor"].Value);
            result.InvertZeroSensor = bool.Parse(xml.Attributes["invertZeroSensor"].Value);
            result.ReverseDirection = bool.Parse(xml.Attributes["reverseDirection"].Value);
            return result;
        }
        
        public void Save(XmlTextWriter xml)
        {
            xml.WriteAttributeString("id", Id);
            xml.WriteAttributeString("description", Description);
            //            xml.WriteAttributeString("enabled", Enabled.ToString());
            xml.WriteAttributeString("stepsFor360", StepsFor360.ToString());
            xml.WriteAttributeString("minStepInterval", MinStepInterval.ToString());
            xml.WriteAttributeString("lastPosition", LastPosition.ToString());
            xml.WriteAttributeString("lastStepDirection", LastStepDirection.ToString());
            xml.WriteAttributeString("lastStepType", LastStepType.ToString());
            xml.WriteAttributeString("lastStepIndex", LastStepIndex.ToString());
            xml.WriteAttributeString("keepTourque", KeepTourque.ToString());
            xml.WriteAttributeString("hasZeroSensor", HasZeroSensor.ToString());
            xml.WriteAttributeString("invertZeroSensor", InvertZeroSensor.ToString());
            xml.WriteAttributeString("reverseDirection", ReverseDirection.ToString());
            _initialPosition = CurrentPosition;
        }
        
        public StepperMotor()
        {
            MotorIndex = 0;
            LastPosition = 0d;
            LastStepDirection = StepDirection.Right;
            LastStepIndex = 3;
            LastStepType = StepType.Full;
        }
        
        public string Id
        {
            get;
            set;
        }
        
        public string Description
        {
            get;
            set;
        }
        
        public int StepsFor360
        {
            get;
            set;
        }
        
        public byte MinStepInterval
        {
            get;
            set;
        }
        
        public double LastPosition
        {
            get;
            set;
        }
        
        public double CurrentPosition
        {
            get { return LastPosition; }
        }
        
        public StepDirection LastStepDirection
        {
            get;
            set;
        }
        
        public StepType LastStepType
        {
            get;
            set;
        }
        
        public int LastStepIndex
        {
            get;
            set;
        }
        
        public bool KeepTourque
        {
            get;
            set;
        }
        
        public bool HasZeroSensor
        {
            get;
            set;
        }
        
        public bool InvertZeroSensor
        {
            get;
            set;
        }
        
        public bool ReverseDirection
        {
            get;
            set;
        }
        
        public virtual bool NeedToSaveState
        {
            get { return _initialPosition != CurrentPosition; }
        }
        
        public StepperDevice Device
        {
            get;
            set;
        }
        
        public int MotorIndex
        {
            get;
            set;
        }
        
        private byte _zeroConfig = 0;
        
        private byte _lastStepConfig = 0;
        
        public string ConfigInfo
        {
            get
            {
                string result = string.Format("Kroki: {0}, dokładność: {1}, czujnik: {2}, kierunek: {3}", StepsFor360, ((double)(360d / (double)StepsFor360)).ToString("0.000"), HasZeroSensor ? (InvertZeroSensor ? "odkryty w 0" : "zakryty w 0") : "brak", ReverseDirection ? "odwrócony" : "normalny");
                return result;
            }
        }
        
        private volatile bool _work = false;
        
        private double _initialPosition = 0d;
        
        public void Initialize()
        {
            Setup();
            
            _work = true;
            
            // uruchomienie wątka wysyłającego rozkazy sterujące silnikiem
            _thread = new Thread(ProcessThread);
            _thread.Start();
        }
        
        private int _fullRoundTime = 0;
        private double _realPositon = 0d;
        private double _stepDegree = 0d;
        
        private void Setup()
        {
            // zapisanie pozycji startowej
            _realPositon = _initialPosition = CurrentPosition;
            
            // wyczyszczenie kolejki rozkazów
            _requests.Clear();
            
            // ustawienie konfiguracji zerowania
            _zeroConfig = (byte)MinStepInterval;
            
            // ustawienie ostatniej pozycji
            // _lastStepConfig
            
            // obliczenie czasu potrzebnego na pełny obrót
            _fullRoundTime = StepsFor360 * MinStepInterval;
            
            // obliczenie ile stopni rusza się oś przy jednym kroku
            _stepDegree = 360d / (double)StepsFor360;
            
            // wysłanie rozkazu RESTORE
            Device.Restore(MotorIndex, _lastStepConfig);
            
            // wyłączenie pauzy
            Device.Pause(MotorIndex, false);
        }
        
        public void Uninitialize()
        {
            _work = false;
            
            // zatrzymanie wątka wysyłającego rozkazy sterujące silnikiem
            if (_thread != null)
            {
                _newRequestEvent.Set();
                try
                {
                    _thread.Join(100);
                }
                catch { }
                if (_thread.ThreadState == ThreadState.Running)
                {
                    try
                    {
                        _thread.Abort();
                    }
                    catch {}
                }
                _thread = null;
            }
        }
        
        private Thread _thread = null;
        
        private void ProcessThread()
        {
            try
            {
                int delay = 0;
                MotorReuqest mr = null;
                DateTime dataStart = DateTime.Now;
                while (_work)
                {
                    mr = null;
                    
                    if ((DateTime.Now - dataStart) >= new TimeSpan(0, 0, 0, 0, delay))
                    {
                        delay = 10;
                        
                        lock (_queueSync)
                        {
                            if (_requests.Count > 0)
                            {
                                mr = _requests.Dequeue();
                            }
                        }
                        
                        if (mr != null)
                        {
                            if (mr.Zero)
                            {
                                if (HasZeroSensor)
                                {
                                    Device.Zero(MotorIndex, mr.Config);
                                    delay = _fullRoundTime;
                                }
                                LastPosition = 0d;
                            }
                            else if (mr.Restore)
                            {
                                Device.Restore(MotorIndex, mr.Config);
                            }
                            else if (mr.Pause)
                            {
                                Device.Pause(MotorIndex, mr.Config > 0);
                            }
                            else
                            {
                                Device.Run(MotorIndex, mr.Config, mr.Steps);
                                LastPosition = mr.NewPosition;
                                delay = mr.Steps * MinStepInterval;
                            }
                        }
                        
                        dataStart = DateTime.Now;
                    }
                    
                    _newRequestEvent.WaitOne(delay);
                    
                    if (!_work)
                    {
                        break;
                    }
                }
            }
            catch (ThreadAbortException)
            {}
            catch (Exception ex)
            {
                Device.Interface.Log.Log(Device.Interface.Module, ex.ToString());
            }
        }
        
        public void SetPause(bool pause)
        {
            lock (_queueSync)
            {
                // wyczyszczenie kolejki rozkazów
                _requests.Clear();
                
                // dodanie rozkazu przywracającego konfigurację
                _requests.Enqueue(new MotorReuqest()
                                  {
                                      Pause = true,
                                      Config = pause ? (byte)1 : (byte)0
                                  });
                
                // powiadomienie o nowym rozkazie
                _newRequestEvent.Set();
            }
        }
        
        public void MarkAsZero()
        {
            Zero();
        }
        
        public void Zero(StepDirection direction)
        {
            _realPositon = 0d;
            
            // ustawienie konfiguracji zerowania
            byte config = _zeroConfig;
            
            if (ReverseDirection)
            {
                if (direction == StepDirection.Left)
                {
                    direction = StepDirection.Right;
                }
                else
                {
                    direction = StepDirection.Left;
                }
            }
            
            if (direction == StepDirection.Right)
            {
                config |= (1 << 7);
            }
            
            lock (_queueSync)
            {
                // wyczyszczenie kolejki rozkazów
                _requests.Clear();
                
                // dodanie rozkazu zerującego
                _requests.Enqueue(new MotorReuqest()
                                  {
                                      Zero = true,
                                      Config = config
                                  });
                
                // powiadomienie o nowym rozkazie
                _newRequestEvent.Set();
            }
        }
        
        public void Zero()
        {
            // sprawdzenie w którą stronę szybciej wypadnie zerowanie
            StepDirection direction = StepDirection.Left;
            
            // sprawdzenie w którą stronę będzie szybciej wyzerować silnik
            if (_realPositon > 180d)
            {
                direction = StepDirection.Right;
            }
            
            if (ReverseDirection)
            {
                if (direction == StepDirection.Left)
                {
                    direction = StepDirection.Right;
                }
                else
                {
                    direction = StepDirection.Left;
                }
            }
            
            Zero(direction);
        }
        
        public void SetPosition(double position, bool? halfStep, bool? right, int? speed)
        {
            if (_realPositon == position)
            {
                return;
            }
            
            bool turnLeft = false;
            
            double k3 = _realPositon - position;
            double k4 = Math.Abs(k3);

            double ileR = 0d;
            if (position >= _realPositon)
            {
                ileR = position - _realPositon;
            }
            else
            {
                ileR = (360d - _realPositon) + position;
            }

            double ileL = 0d;
            if (position >= _realPositon)
            {
                ileL = (360d - position) + _realPositon;
            }
            else
            {
                ileL = position - _realPositon;
            }
            ileL = Math.Abs(ileL);

            bool prawo = ileR <= ileL;
            
            if (right == null)
            {
                // wykrycie krótszego ruchu
                right = prawo;
            }
            
            int steps = 0;
            double newPosition = _realPositon;

            if (right.Value)
            {
                // ruch w prawo
                steps = (int)(ileR / _stepDegree);
                
                newPosition += (double)steps * _stepDegree;
            }
            else
            {
                // ruch w lewo
                turnLeft = true;
                
                steps = (int)(ileL / _stepDegree);
                
                newPosition -= (double)steps * _stepDegree;
            }
            
            if (newPosition < 0d)
            {
                newPosition += 360d;
            }
            if (newPosition >= 360d)
            {
                newPosition -= 360d;
            }
            
            _realPositon = newPosition;
            
            if (ReverseDirection)
            {
                turnLeft = !turnLeft;
            }
            
            byte config = MinStepInterval;
            
            if (speed.HasValue)
            {
                if (speed.Value > 0 && speed.Value <= 127)
                {
                    config = (byte)speed.Value;
                }
            }
            
            if (turnLeft)
            {
                config |= (1 << 7);
            }
            
            lock (_queueSync)
            {
                // dodanie rozkazu ustawiającego pozycję
                _requests.Enqueue(new MotorReuqest()
                                  {
                                      Config = config,
                                      Steps = steps,
                                      NewPosition = newPosition
                                  });
                
                // powiadomienie o nowym rozkazie
                _newRequestEvent.Set();
            }
        }
        
        public void StopMotor()
        {
        	
        }
        
        private class MotorReuqest
        {
            public bool Zero = false;
            public bool Restore = false;
            public bool Pause = false;
            public int Steps = 0;
            public byte Config = 0;
            public double NewPosition = 0d;
        }
        
        private object _queueSync = new object();
        private Queue<MotorReuqest> _requests = new Queue<MotorReuqest>();
        private AutoResetEvent _newRequestEvent = new AutoResetEvent(true);
    }
}
