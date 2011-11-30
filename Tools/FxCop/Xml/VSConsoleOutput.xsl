<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/TR/xhtml1/strict">

<xsl:output method="text"/>

<xsl:template match="/">
	<xsl:apply-templates select="//Issue" />
	<xsl:apply-templates select="//Exception" />
</xsl:template>

<xsl:template match="Exception"><xsl:value-of select="/FxCopReport/Localized/String[@Key='Project']/text()" /> : <xsl:variable name="kind" select="@Kind" /> <xsl:if test="not(@TreatAsWarning)">error</xsl:if><xsl:if test="@TreatAsWarning">warning</xsl:if> : <xsl:value-of select="@Keyword" /> : <xsl:if test="@CheckId"><xsl:value-of select="/FxCopReport/Localized/String[@Key='Rule']/text()" />=<xsl:value-of select="@Category" />#<xsl:value-of select="@CheckId" />, <xsl:value-of select="/FxCopReport/Localized/String[@Key='Target']/text()" />=<xsl:value-of select="@Target" /> : </xsl:if><xsl:value-of select="ExceptionMessage/text()" /><xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
</xsl:template>

<xsl:template match="Issue">
	<xsl:if test="@Path"><xsl:value-of select="@Path"/>\<xsl:value-of select="@File"/>(<xsl:value-of select="@Line"/>,1) : </xsl:if><xsl:if test="not(@Path)"><xsl:value-of select="/FxCopReport/Localized/String[@Key='LocationNotStoredInPdb']/text()" /> : </xsl:if><xsl:if test="../@BreaksBuild">error </xsl:if><xsl:if test="not(../@BreaksBuild)">warning </xsl:if> : <xsl:apply-templates select=".." mode="parentMessage" /> : <xsl:value-of select="text()" /><xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
</xsl:template>

<xsl:template match="Message" mode="parentMessage">	
	<xsl:value-of select="@CheckId"/> : <xsl:value-of select="@Category"/>
</xsl:template>


<xsl:template match="Text">
	<xsl:value-of select="translate(normalize-space(text()),':','')"/>
</xsl:template>

<xsl:template match="Rules/Rule"/>
<xsl:template match="Note"/>

			
</xsl:stylesheet>


