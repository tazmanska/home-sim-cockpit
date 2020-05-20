/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-13
 * Godzina: 21:11
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Xml;

namespace RS232HCDevices.Steppers
{
    /// <summary>
    /// Description of SimpleStepperDevice.
    /// </summary>
    class SimpleStepperDevice : StepperDevice
    {
        public static readonly byte RESET = 0x01;
        public static readonly byte RUN_MOTOR_1 = 0x02;
        public static readonly byte RUN_MOTOR_2 = 0x03;
        public static readonly byte PAUSE = 0x04;
        public static readonly byte ZERO_MOTOR_1 = 0x05;
        public static readonly byte ZERO_MOTOR_2 = 0x06;
        public static readonly byte RESTORE_MOTOR_1 = 0x07;
        public static readonly byte RESTORE_MOTOR_2 = 0x08;
        public static readonly byte CONFIG = 0x09;
        
        public SimpleStepperDevice()
        {
        }
        
        public override void Load(XmlNode xml)
        {
            Motor1 = StepperMotor.Load(xml.SelectSingleNode("motor1"));
            if (Motor1 != null)
            {
                Motor1.MotorIndex = 0;
                Motor1.Device = this;
            }
            Motor2 = StepperMotor.Load(xml.SelectSingleNode("motor2"));
            if (Motor2 != null)
            {
                Motor2.MotorIndex = 1;
                Motor2.Device = this;
            }
        }
        
        private int _pause = 0;
        
        public override void Initialize()
        {
            // RESET stanu silników
            Interface.Write(new byte[] { DeviceId, 1, RESET });
            
            // odczytanie konfiguracji
            byte config = 0;
            if (Motor1 != null)
            {
                if (Motor1.KeepTourque)
                {
                    config |= 0x01;
                }
                if (Motor1.InvertZeroSensor)
                {                    
                    config |= 0x04;
                }
            }
            if (Motor2 != null)
            {
                if (Motor2.KeepTourque)
                {
                    config |= 0x02;
                }
                if (Motor2.InvertZeroSensor)
                {
                    config |= 0x08;
                }
            }
            
            // CONFIG konfiguracja silników
            Interface.Write(new byte[] { DeviceId, 2, CONFIG, config });
            
            _pause = 1 | 2;
            
            if (Motor1 != null)
            {
                Motor1.Device = this;
                Motor1.Initialize();
            }
            
            if (Motor2 != null)
            {
                Motor2.Device = this;
                Motor2.Initialize();
            }
            
        }
        
        public override void Uninitialize()
        {
            if (Motor1 != null)
            {
                Motor1.Uninitialize();                
            }
            
            if (Motor2 != null)
            {
                Motor2.Uninitialize();
            }
            
            // RESET
            Interface.Write(new byte[] { DeviceId, 1, RESET });
            
            // zapisanie stanu
            if (NeedToSaveState)
            {
                XMLConfiguration.Load().Save();
            }
        }
        
        public override StepperDeviceType Type
        {
            get { return StepperDeviceType.Simple; }
        }
        
        public override void Pause(int motorIndex, bool pause)
        {
            if (pause)
            {
                _pause |= (1 << motorIndex);
            }
            else
            {
                _pause &= ~(1 << motorIndex);
            }
            Interface.Write(new byte[] { DeviceId, 2, PAUSE, (byte)_pause });
        }
        
        public override void Zero(int motorIndex, byte config)
        {
            Interface.Write(new byte[] { DeviceId, 2, motorIndex == 0 ? ZERO_MOTOR_1 : ZERO_MOTOR_2, config } );
        }
        
        public override void Restore(int motorIndex, byte lastStep)
        {
            Interface.Write(new byte[] { DeviceId, 2, motorIndex == 0 ? RESTORE_MOTOR_1 : RESTORE_MOTOR_2, lastStep });
        }
        
        public override void Run(int motorIndex, byte config, int steps)
        {
            Interface.Write(new byte[] { DeviceId, 4, motorIndex == 0 ? RUN_MOTOR_1 : RUN_MOTOR_2, config, (byte)((steps >> 8) & 0xff), (byte)(steps & 0xff) });
        }
    }
}