<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/TR/xhtml1/strict">

<xsl:output method="text"/>
<xsl:template match="/">
    <xsl:apply-templates select="//Message" />
</xsl:template>

<xsl:template match="Message">
    <xsl:variable name="typeName" select ="@TypeName" />
<xsl:if test="not(../../self::Module)"><xsl:apply-templates select=".." mode="module"/></xsl:if><xsl:apply-templates select=".." mode="parent"/>
=========================================================================
              Rule: <xsl:value-of select="/FxCopReport/Rules/Rule[@TypeName=$typeName]/Name/text()" />
     Documentation: <xsl:value-of select="/FxCopReport/Rules/Rule[@TypeName=$typeName]/Url/text()" />
          Category: <xsl:value-of select="@Category" />
          Check Id: <xsl:value-of select="@CheckId" />
<xsl:if test="@Id">
        Message Id: <xsl:value-of select="@Id" /></xsl:if>  
       Suppression: <xsl:choose>
            <xsl:when test="../../self::Resource"><xsl:apply-templates select="." mode="moduleSuppression"/></xsl:when>
            <xsl:when test="../../self::Namespace"><xsl:apply-templates select="." mode="moduleSuppression"/></xsl:when>
            <xsl:otherwise>[<xsl:if test="../../self::Module">module: </xsl:if>SuppressMessage("<xsl:value-of select="@Category"/>", "<xsl:value-of select="@CheckId"/>:<xsl:value-of select="@TypeName"/>")]</xsl:otherwise>
          </xsl:choose>
<xsl:choose><xsl:when test="../../self::Resource"></xsl:when>
            <xsl:when test="../../self::Namespace"></xsl:when>
            <xsl:when test="../../self::Module"></xsl:when>
            <xsl:otherwise>
Module Suppression: <xsl:apply-templates select="." mode="moduleSuppression"/></xsl:otherwise>
          </xsl:choose><xsl:if test="count(./Issue)>1">

 Issues (<xsl:value-of select="count(./Issue)"/>)
 -----------</xsl:if><xsl:apply-templates select="Issue"/>
<xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
<xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
<xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
</xsl:template>

<xsl:template match="Issue">
     Message Level: <xsl:value-of select="@Level" />
         Certainty: <xsl:value-of select="@Certainty" />%<xsl:if test="@Path">
       Source Code: <xsl:value-of select="@Path" />\<xsl:value-of select="@File" />(<xsl:value-of select="@Line"/>)</xsl:if>       
        Resolution: <xsl:value-of select="text()" />
<xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
</xsl:template>

<xsl:template match="Messages" mode="parent">
    <xsl:apply-templates select=".." mode="parent" />
</xsl:template>

<xsl:template match="Namespace" mode="parent">
   <xsl:value-of select="@Name" />
</xsl:template>

<xsl:template match="Module" mode="parent">
    <xsl:value-of select="@Name" />
</xsl:template>

<xsl:template match="Target" mode="parent">
    <xsl:value-of select="@Name" />
</xsl:template>

<xsl:template match="Accessor" mode="parent">
    <xsl:apply-templates select="../../.." mode="parent" />.<xsl:value-of select="@Name" />    
</xsl:template>

<xsl:template match="Member" mode="parent">
    <xsl:apply-templates select=".." mode="parent" />.<xsl:value-of select="@Name" />
</xsl:template>

<xsl:template match="Resource" mode="parent">
    <xsl:value-of select="@Name" />
</xsl:template>

<xsl:template match="Type" mode="parent">
    <xsl:if test="not(../../@Name='')"><xsl:apply-templates select=".." mode="parent" />.</xsl:if><xsl:value-of select="@Name" />
</xsl:template>

<xsl:template match="*" mode="parent">
   <xsl:apply-templates select=".." mode="parent" />
</xsl:template>

<xsl:template match="FxCopReport" mode="parent">
</xsl:template>


<xsl:template match="Module" mode="module">[<xsl:value-of select="@Name"/>] </xsl:template>

<xsl:template match="*" mode="module"><xsl:apply-templates select=".." mode="module" /></xsl:template>

<xsl:template match="FxCopReport" mode="module">
</xsl:template>

<xsl:template match="Message" mode="moduleSuppression">[module: SuppressMessage("<xsl:value-of select="@Category"/>", "<xsl:value-of select="@CheckId"/>:<xsl:value-of select="@TypeName"/>", Scope="<xsl:apply-templates select="../.." mode="suppressionScope" />", Target="<xsl:apply-templates select=".." mode="parent"/>"<xsl:if test="ancestor::Resource"><xsl:if test="@Id">, MessageId="<xsl:value-of select="@Id" />"</xsl:if></xsl:if>)]</xsl:template>

<xsl:template match="*" mode="suppressionScope">
        <xsl:choose>            
            <xsl:when test="self::Resource">resource</xsl:when>
            <xsl:when test="self::Namespace">namespace</xsl:when>
            <xsl:when test="self::Type">type</xsl:when>
            <xsl:when test="self::Member">member</xsl:when>
            <xsl:when test="self::Accessor">member</xsl:when>
        </xsl:choose>
</xsl:template>
</xsl:stylesheet>
