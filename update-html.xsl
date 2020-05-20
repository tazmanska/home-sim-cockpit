<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <html>
  <body>
    <h1>HomeSimCockpit update</h1>
      <h2>Cycle: <xsl:value-of select="update/@cycle"/> (<xsl:value-of select="update/@date"/>)</h2>
    <table border="1">
      <tr bgcolor="#9acd32">
        <th>File name</th>
        <th>Version</th>
      </tr>
      <xsl:for-each select="update/files/addFile">
            <xsl:sort select="@name"/>
        <tr>
          <td><xsl:value-of select="@name"/></td>
          <td><xsl:value-of select="@version"/></td>
        </tr>
      </xsl:for-each>
    </table>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>