#***************************************************************************
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
# ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
# IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
# PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
#
#***************************************************************************


#
#  This script will convert a ctc into a vsct
#
#  It will not process conditionals.  If your file has #if... or #else directives they will be
#  marked in comments in the xml but will not be processed in to Conditional attributes.
#
#  Line comments will be ignored however block comments i.e. /*   */ will not be marked.
#  
#  After processing with this script, examine the result for errors conditional, directives,
#  and block comments making hand edits for conditionals and block comments.
#
#


# flags for sections being read
$bCommands = 0;
$bMenus = 0;
$bGroups = 0;
$bPlacements = 0;
$bKeyBindings = 0;
$bBitmaps = 0;
$bButtons = 0;
$bCombos = 0;
$bVisibilities = 0;
$bUsed = 0;

# Map values to new casing
@Flags = (
"NoCustomize",
"NoKeyCustomize",
"NoButtonCustomize",
"TextContextUseButton",
"TextChangesButton",
"TextChanges",
"DefaultDisabled",
"DefaultInvisible",
"DynamicVisibility",
"Repeat",
"DynamicItemStart",
"CommandWellOnly",
"Pict",
"TextOnly",
"IconAndText",
"AllowParams",
"FilterKeys",
"PostExec",
"DontCache",
"FixMenuController",
"NoShowOnMenuController",
"RouteToDocs",
"NoAutoComplete",
"TextMenuUseButton",
"TextMenuCtrlUseMenu",
"TextCascadeUseButton",
"CaseSensitive",
"DefaultDocked",
"NoToolbarClose",
"NotInTBList",
"AlwaysCreate",
"TextIsAnchorCommand",
"StretchHorizontally",
"ProfferedCmd",

"Separator",
"Button",
"MenuButton",
"Swatch",
"SplitDropDown",
"DropDownCombo",
"MRUCombo",
"DynamicCombo",
"OwnerDrawCombo",
"IndexCombo",
"Menu",
"MenuController",
"Toolbar",
"Context",
"ToolWindowToolbar",
"MenuControllerLatched",
"Shared",
"AppID",
"DefaultDocked",
"NotToolbarClose",
"NotInTBList",
"AlwaysCreate"
);


# these mappings are for common combinations of flags usualy appearing as #define macros
# these must be expanded first before splitting the flags of buttons, combos or menus
$MacroFlags{"BTN_FLAGS"}="DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED";
$MacroFlags{"BTNDOCS_FLAGS"}="DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED|ROUTETODOCS";
$MacroFlags{"ZOOM_FLAGS"}="DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED|ROUTETODOCS|FIXMENUCONTROLLER|NOSHOWONMENUCONTROLLER";
$MacroFlags{"BTNTXT_FLAGS"}="DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED|TEXTCHANGESBUTTON|TEXTCHANGES";
$MacroFlags{"CWO_FLAGS"}="COMMANDWELLONLY|DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED";
$MacroFlags{"CWOTXT_FLAGS"}="COMMANDWELLONLY|DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED|TEXTCHANGES";
$MacroFlags{"COMBO_FLAGS"}="DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED";
$MacroFlags{"ADDSYM_FLAGS"}="DYNAMICVISIBILITY|DEFAULTINVISIBLE|DEFAULTDISABLED|TEXTCHANGES|TEXTCHANGESBUTTON";
$MacroFlags{"DIS_DEF"}="DEFAULTDISABLED | DEFAULTINVISIBLE | DYNAMICVISIBILITY";
$MacroFlags{"VIS_DEF"}="COMMANDWELLONLY";
$MacroFlags{"TBR_DEF"}="TOOLBAR | ALWAYSCREATE | DEFAULTDOCKED";

foreach $flag (@Flags)
{
    $upperFlag = ToUpper($flag);
    $flagMap{$upperFlag} = $flag;
    $flagMap{$flag} = $flag;
}

foreach $key (%flagMap)
{
    $value = $flagMap{$key};
}







$source = $ARGV[0];
$dest = $ARGV[1];
if ($dest eq "")
{
    #default target file with the .vsct extension
    $dest = "$source.vsct";
}

print "Converting \"$source\" to VSCT\n";


#open the source file
open (INPUT, $source) || die "Could not open $source\n";


# open the target file
open (VSCT, ">$dest");

print VSCT "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
print VSCT "<CommandTable xmlns=\"http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">\n";
ConvertFile();
print VSCT "</CommandTable>\n";
close VSCT;
close INPUT;




sub Trim
{
    $text = shift(@_);
    for ($text)
    {           # trim white space in $variable, cheap
        s/^\s+//;
        s/\s+$//;
    }
    return $text;
}

sub TrimEnd
{
    $text = shift(@_);
    for ($text)
    {           # trim white space in $variable, cheap
        s/\s+$//;
    }
    return $text;
}

sub TrimQuotes
{
    $text = Trim(shift(@_));
    for ($text)
    {
        s/^[\"\']//;
        s/[\"\']$//;
    }
    return $text;
}

sub EscapeChars
{
    $text = shift(@_);
    for ($text)
    {
        s/\&/\[amp\]/g;
        s/\[amp\]/&amp\;/g;
        s/</&lt;/g;
        s/>/&gt;/g;
    }
    return $text
}

sub WriteStrings
{
    my $strings = Trim(shift(@_));
    # split up the strings portion taking into acount that the old compiler didn't even require them to be comma separated
    if ($strings =~ /(\"([^\"]*)\"|\s*)(\s*,?\s*(\"([^\"]*)\"|\s*))?(\s*,?\s*(\"([^\"]*)\"|\s*))?(\s*,?\s*(\"([^\"]*)\"|\s*))?(\s*,?\s*(\"([^\"]*)\"|\s*))?(\s*,?\s*(\"([^\"]*)\"|\s*))?/)
    {
        $buttonText = EscapeChars($2);
        $menuText = EscapeChars($5);
        $tipText = EscapeChars($8);
        $commandName = EscapeChars($11);
        $canonicalName = EscapeChars($14);
        $locName = EscapeChars($17);
        
        if ($bMenus)
        {
            $locName = $commandName;
            $canonicalName = $tipText;
            $commandName = $menuText;;
            
            $menuText = ""; # menus don't use these fields
            $tipText = "";            
        }

        print VSCT "            <Strings>\n";
        if ($buttonText ne "")
        {
            print VSCT "                <ButtonText>$buttonText</ButtonText>\n";
        }
        if ($menuText ne "")
        {
            print VSCT "                <MenuText>$menuText</MenuText>\n";
        }
        if ($tipText ne "")
        {
            print VSCT "                <ToolTipText>$tipText</ToolTipText>\n";
        }
        if ($commandName ne "")
        {
            print VSCT "                <CommandName>$commandName</CommandName>\n";
        }
        if ($canonicalName ne "")
        {
            print VSCT "                <CanonicalName>$canonicalName</CanonicalName>\n";
        }
        if ($locName ne "")
        {
            print VSCT "                <LocCanonicalName>$locName</LocCanonicalName>\n";
        }
        print VSCT "            </Strings>\n";
    }
}

sub ToUpper
{
    $text = Trim(shift(@_));
    for ($text)
    {
        tr/[a-z]/[A-Z]/;
    }
    return $text
}


sub ConvertFile()
{
    while (<INPUT>)
    {
        chomp;
        $line = TrimEnd($_);
        
        # if this line does not begin as a comment but it does contain one, just remove it
        if (!($line =~ /^\s*\/\//) && ($line =~ /\/\//))
        {
            $line =~ s/\s*\/\/.*//;
        }

        SetSection();
        # if a line does not begin as a comment and it ends in a semicoln we need to parse it as a CTC item
        # dump in place and process as comments
        if ($line =~ /;$/ && !($line =~ /^\s*\/\//) ) # everything we want ends in a semicoln
        {
            chop($line);
            if ($bButtons)
            {
                @tokens = split(/,/, $line);
                
                $scope = $tokens[0];
                $parent = $tokens[1];
                
                $scope =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);

                $parent =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $parentGuid = Trim($1);
                $parentID = Trim($2);
                
                $priority = Trim($tokens[2]);
                if (!($priority =~ /^\d/))
                {
                    print VSCT "<!-- ERROR priority not a number, possible bad format: $line -->\n";
                }
                
                $icon = Trim($tokens[3]);
                if ($icon =~ /(NOICON|NO_ICON)/)
                {
                    $iconGuid = "";
                }
                elsif ($icon =~ /MSO_ICON\((.*)\)/)
                {
                    $iconGuid = "guidOfficeIcon";
                    $iconID = "msotcid$1";
                }
                elsif ($icon =~ /HAT_ICON\((.*)\)/)
                {
                    $iconGuid = "guidHatPackageCmdSet";
                    $iconID = "icon$1";
                }
                else
                {
                    $icon =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                    $iconGuid = Trim($1);
                    $iconID = Trim($2);
                    if ($iconGuid eq "guidOfficeIcon" && $iconID eq "msotcidNoIcon")
                    {
                        $iconGuid = "";
                    }
                }
                
                $type = Trim($tokens[4]);
                $flags = Trim($tokens[5]);
                print "Format error \n" if (!(Trim($tokens[6]) =~ /^\"/));
                
                @buttonflags = split(/\|/, $flags);
                # preprocess flas to get rid of macros and resplit the list
                foreach $flag (@buttonflags)
                {
                    $flag = Trim($flag);
                    if ($MacroFlags{$flag} ne "")
                    {
                        $flags =~ s/$flag/$MacroFlags{$flag}/;
                    }
                }
                @buttonflags = split(/\|/, $flags);
                
                if ($type eq "")
                {
                    $type = "Button";
                }
                
                $typeUpper = ToUpper($type);
                $type = $flagMap{ $typeUpper };

                print VSCT "        <Button guid=\"$scopeGuid\" id=\"$scopeID\" priority=\"$priority\" type=\"$type\">\n";

                # if the parent is itself or group undefined or in the same guid group with id==0 skip it
                # in the xml parent definition is optional
                if ($parentGuid ne "Group_Undefined")
                {
                    print VSCT "            <Parent guid=\"$parentGuid\" id=\"$parentID\"\/>\n"
                }

                if ($iconGuid ne "Group_Undefined" && $iconGuid ne "")
                {
                    print VSCT "            <Icon guid=\"$iconGuid\" id=\"$iconID\"\/>\n"
                }
                
                foreach $flag (@buttonflags)
                {
                    $flag = Trim($flag);
                    $flagUpper = ToUpper($flag);
                    $flag = $flagMap{$flagUpper};
                    if ($flag ne "")
                    {
                        print VSCT "            <CommandFlag>$flag</CommandFlag>\n";
                    }
                }
                
                $line =~ /[^\"]*(\".*)/;
                $strings = $1;
                WriteStrings($strings);

                print VSCT "        </Button>\n";
            }
            elsif ($bCombos)
            {
                @tokens = split(/,/, $line);
                
                $scope = $tokens[0];
                $parent = $tokens[1];
                
                $scope =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);

                $parent =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $parentGuid = Trim($1);
                $parentID = Trim($2);
                
                $priority = Trim($tokens[2]);
                if (!($priority =~ /^\d/))
                {
                    print VSCT "<!-- ERROR priority not a number, possible bad format: $line -->\n";
                }
                
                $getListCmd = Trim($tokens[3]);
                $defaultWidth = Trim($tokens[4]);
                
                $type = Trim($tokens[5]);
                $flags = Trim($tokens[6]);
                print "Format error \n" if (!(Trim($tokens[7]) =~ /^\"/));

                @buttonflags = split(/\|/, $flags);
                # preprocess flas to get rid of macros and resplit the list
                foreach $flag (@buttonflags)
                {
                    $flag = Trim($flag);
                    if ($MacroFlags{$flag} ne "")
                    {
                        $flags =~ s/$flag/$MacroFlags{$flag}/;
                    }
                }
                @buttonflags = split(/\|/, $flags);
                
                if ($type eq "")
                {
                    $type = "DynamicCombo";
                }
                
                $typeUpper = ToUpper($type);
                $type = $flagMap{ $typeUpper };

                print VSCT "        <Combo guid=\"$scopeGuid\" id=\"$scopeID\" priority=\"$priority\" type=\"$type\" defaultWidth=\"$defaultWidth\" idCommandList=\"$getListCmd\">\n";

                # if the parent is itself or group undefined or in the same guid group with id==0 skip it
                # in the xml parent definition is optional
                if ($parentGuid ne "Group_Undefined")
                {
                    print VSCT "            <Parent guid=\"$parentGuid\" id=\"$parentID\"\/>\n"
                }

                if ($iconGuid ne "Group_Undefined" && $iconGuid ne "")
                {
                    print VSCT "            <Icon guid=\"$iconGuid\" id=\"$iconID\"\/>\n"
                }

                foreach $flag (@buttonflags)
                {
                    $flag = Trim($flag);
                    $flagUpper = ToUpper($flag);
                    $flag = $flagMap{$flagUpper};
                    if ($flag ne "")
                    {
                        print VSCT "            <CommandFlag>$flag</CommandFlag>\n";
                    }
                }
                
                $line =~ /[^\"]*(\".*)/;
                $strings = $1;
                WriteStrings($strings);

                print VSCT "        </Combo>\n";
            }
            elsif ($bMenus)
            {
                @tokens = split(/,/, $line);
                
                $scope = $tokens[0];
                $parent = $tokens[1];
                
                $scope =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);

                $parent =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $parentGuid = Trim($1);
                $parentID = Trim($2);
                
                $priority = Trim($tokens[2]);
                if (!($priority =~ /^\d/))
                {
                    print VSCT "<!-- ERROR priority not a number, possible bad format: $line -->\n";
                }
                
                $flags = Trim($tokens[3]);
                print "Format error \n" if (!(Trim($tokens[4]) =~ /^\"/));
                
                if ($flags =~ /.*(Context|MenuControllerLatched|ToolWindowToolbar).*/i)
                {
                    $type = $1;
                }
                elsif ($flags =~ /.*(MenuController).*/i)
                {
                    $type = $1;
                }
                # get these in a second pass so they will not match first
                elsif ($flags =~ /.*(Menu|Toolbar).*/i)
                {
                    $type = $1;
                }
                else
                {
                    $type = "";
                }
                

                @flags = split(/\|/, $flags);

                if ($type eq "")
                {
                    $type = "Menu";
                }
                
                $typeUpper = ToUpper($type);
                $type = $flagMap{ $typeUpper };

                print VSCT "        <Menu guid=\"$scopeGuid\" id=\"$scopeID\" priority=\"$priority\" type=\"$type\">\n";

                # if the parent is itself or group undefined or in the same guid group with id==0 skip it
                # in the xml parent definition is optional
                if ($parentGuid ne "Group_Undefined")
                {
                    print VSCT "            <Parent guid=\"$parentGuid\" id=\"$parentID\"\/>\n"
                }

                foreach $flag (@flags)
                {
                    $flag = Trim($flag);
                    $flagUpper = ToUpper($flag);
                    $flag = $flagMap{$flagUpper};
                    if ($flag ne "" && $flag ne $type)
                    {
                        print VSCT "            <CommandFlag>$flag</CommandFlag>\n";
                    }
                }
                
                $line =~ /[^\"]*(\".*)/;
                $strings = $1;
                WriteStrings($strings);

                print VSCT "        </Menu>\n";
            }
            elsif ($bGroups)
            {
                @tokens = split(/,/, $line);
                $tokenCount = @tokens;
                if ($tokenCount < 3)
                {
                    print VSCT "<!-- ERROR Missing Token ($tokenCount): $line -->\n";
                }
                
                $scope = $tokens[0];
                $parent = $tokens[1];
                
                $scope =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);

                $parent =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $parentGuid = Trim($1);
                $parentID = Trim($2);
                
                $priority = Trim($tokens[2]);
                $dynamic = $tokens[3];
                
                if ($dynamic ne "" || ($parentGuid ne "Group_Undefined"))
                {
                    print VSCT "        <Group guid=\"$scopeGuid\" id=\"$scopeID\" priority=\"$priority\">\n";
                    # if the parent is itself or group undefined or in the same guid group with id==0 skip it
                    # in the xml parent definition is optional
                    if ($parentGuid ne "Group_Undefined")
                    {
                        print VSCT "            <Parent guid=\"$parentGuid\" id=\"$parentID\"\/>\n"
                    }

                    if ($dynamic ne "")
                    {
                        print VSCT "            <GroupFlag>Dynamic</GroupFlag>\n";
                    }

                    print VSCT "        </Group>\n";
                }
                else
                {
                    print VSCT "        <Group guid=\"$scopeGuid\" id=\"$scopeID\" priority=\"$priority\"/>\n";
                }
                    
            }
            elsif ($bBitmaps)
            {
                $line =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*,\s*(.*)$/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);
                $usedList = $3;

                print VSCT "    <Bitmap guid=\"$scopeGuid\" resID=\"$scopeID\" usedList=\"$usedList\"/>\n";
            }
            elsif ($bPlacements)
            {
                @tokens = split(/,/, $line);
                $tokenCount = @tokens;
                if ($tokenCount != 3)
                {
                    print VSCT "<!-- ERROR Missing Token ($tokenCount): $line -->\n";
                }
                
                $scope = $tokens[0];
                $parent = $tokens[1];
                
                $scope =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);

                $parent =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $parentGuid = Trim($1);
                $parentID = Trim($2);
                
                $priority = Trim($tokens[2]);
                
                print VSCT "        <CommandPlacement guid=\"$scopeGuid\" id=\"$scopeID\" priority=\"$priority\">\n";
                if ($parentGuid ne $scopeGuid || $parentID ne $scopeID)
                {
                    print VSCT "            <Parent guid=\"$parentGuid\" id=\"$parentID\"\/>\n"
                }
                print VSCT "        </CommandPlacement>\n";
            }
            elsif ($bVisibilities)
            {
                @tokens = split(/,/, $line);
                $tokenCount = @tokens;
                if ($tokenCount != 2)
                {
                    print VSCT "<!-- ERROR Missing Token ($tokenCount): $line -->\n";
                }
                
                $scope = $tokens[0];
                $context = Trim($tokens[1]);
                
                $scope =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);

                print VSCT "        <VisibilityItem guid=\"$scopeGuid\" id=\"$scopeID\" context=\"$context\"/>\n";
            }
            elsif ($bKeyBindings)
            {
                @tokens = split(/,/, $line);
                $tokenCount = @tokens;
                if ($tokenCount != 4)
                {
                    print VSCT "<!-- ERROR Missing Token ($tokenCount): $line -->\n";
                }
                
                $scope = $tokens[0];
                $editor = Trim($tokens[1]);
                $emulator = Trim($tokens[2]);
                $chord = Trim($tokens[3]);
                
                $scope =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);
                
                @keys = split(/:/, $chord);
                
                $key1 = TrimQuotes($keys[0]);
                $mod1 = Trim($keys[1]);
                
                $key2 = TrimQuotes($keys[2]);
                $mod2 = Trim($keys[3]);

                $key1 =~ tr/a-z/A-Z/;
                $key2 =~ tr/a-z/A-Z/;
                
                $mod1 = join(' ', split(//, $mod1));
                $mod2 = join(' ', split(//, $mod2));
                
                $mod1 =~ s/C/Control/i;
                $mod1 =~ s/A/Alt/i;
                $mod1 =~ s/S/Shift/i;
                
                $mod2 =~ s/C/Control/i;
                $mod2 =~ s/A/Alt/i;
                $mod2 =~ s/S/Shift/i;

                print VSCT "        <KeyBinding guid=\"$scopeGuid\" id=\"$scopeID\" editor=\"$editor\"";
                if ($editor ne $emulator)
                {
                    print VSCT " emulator=\"$emulator\"";
                }
                if ($key1 ne "")
                {
                    print VSCT " key1=\"$key1\"";
                }
                if ($mod1 ne "")
                {
                    print VSCT " mod1=\"$mod1\"";
                }
                if ($key2 ne "")
                {
                    print VSCT " key2=\"$key2\"";
                }
                if ($mod2 ne "")
                {
                    print VSCT " mod2=\"$mod2\"";
                }
                
                print VSCT "/>\n";
            }
            elsif ($bUsed)
            {
                $line =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $scopeGuid = Trim($1);
                $scopeID = Trim($2);

                $parent =~ /\s*([^\s,]*)\s*:\s*([^\s,]*)\s*/;
                $parentGuid = Trim($1);
                $parentID = Trim($2);
                
                print VSCT "    <UsedCommand guid=\"$scopeGuid\" id=\"$scopeID\"/>\n";
            }
        }
        else
        {
            if (/^\s*#include\s*[\"\'<]([^\"].*)[\"\'>]/)
            {
				$includefile = $1;
				if ($includefile =~ /\.ctc/)
				{
					$includefile =~ s/\.ctc/\.vsct/;
					print VSCT "<Include href=\"$includefile\"/>\n";
				}
				else
				{
					print VSCT "<Extern href=\"$includefile\"/>\n";
                }
            }
            elsif (/#if/ || /#else/)
            {
                print VSCT "<!-- Make conditional for the items in this block -->\n";
            }

            s/--/**/g; # "--" is ilegal in xml comments
            print VSCT "<!-- $_ -->\n";
        }
        
    }
}
    
sub SetSection()
{
    if (/^\s*CMDS_SECTION/)
    {
        /CMDS_SECTION\s*([^\s]*)/;
        $bCommands = 1;
        if ($1 ne "")
        {
            print VSCT "<Commands package=\"$1\">\n";
        }
        else
        {
            print VSCT "<Commands>\n";
        }
    }
    elsif (/^\s*CMDS_END/)
    {
        $bCommands = 0;
        print VSCT "</Commands>\n";
    }
    elsif (/^\s*(APPID|SHARED)?(_)?BUTTONS_BEGIN/)
    {
        $bButtons = 1;
        if ($1 eq "APPID")
        {
            $tag = "AppID";
        }
        elsif ($1 eq "SHARED")
        {
            $tag = "Shared";
        }
        else
        {
            $tag = "";
        }
        print VSCT "    <$tag";
        print VSCT "Buttons>\n";
    }
    elsif (/^\s*(APPID|SHARED)?_?BUTTONS_END/)
    {
        $bButtons = 0;
        if ($1 eq "APPID")
        {
            $tag = "AppID";
        }
        elsif ($1 eq "SHARED")
        {
            $tag = "Shared";
        }
        else
        {
            $tag = "";
        }
        print VSCT "    </$tag";
        print VSCT "Buttons>\n";
    }
    elsif (/^\s*(APPID|SHARED)?(_)?COMBOS_BEGIN/)
    {
        $bCombos = 1;
        if ($1 eq "APPID")
        {
            $tag = "AppID";
        }
        elsif ($1 eq "SHARED")
        {
            $tag = "Shared";
        }
        else
        {
            $tag = "";
        }
        
        print VSCT "    <$tag";
        print VSCT "Combos>\n";
    }
    elsif (/^\s*(APPID|SHARED)?_?COMBOS_END/)
    {
        $bCombos = 0;
        if ($1 eq "APPID")
        {
            $tag = "AppID";
        }
        elsif ($1 eq "SHARED")
        {
            $tag = "Shared";
        }
        else
        {
            $tag = "";
        }

        print VSCT "    </$tag";
        print VSCT "Combos>\n";
    }
    elsif (/^\s*MENUS_BEGIN/)
    {
        $bMenus = 1;
        print VSCT "    <Menus>\n";
    }
    elsif (/^\s*MENUS_END/)
    {
        $bMenus = 0;
        print VSCT "    </Menus>\n";
    }
    elsif (/^\s*NEWGROUPS_BEGIN/)
    {
        $bGroups = 1;
        print VSCT "    <Groups>\n";
    }
    elsif (/^\s*NEWGROUPS_END/)
    {
        $bGroups = 0;
        print VSCT "    </Groups>\n";
    }
    elsif (/^\s*BITMAPS_BEGIN/)
    {
        $bBitmaps = 1;
        print VSCT "<Bitmaps>\n";
    }
    elsif (/^\s*BITMAPS_END/)
    {
        $bBitmaps = 0;
        print VSCT "</Bitmaps>\n";
    }
    elsif (/^\s*CMDPLACEMENT_SECTION/)
    {
        $bPlacements = 1;
        print VSCT "<CommandPlacements>\n";
    }
    elsif (/^\s*CMDPLACEMENT_END/)
    {
        $bPlacements = 0;
        print VSCT "</CommandPlacements>\n";
    }
    elsif (/^\s*VISIBILITY_SECTION/)
    {
        $bVisibilities = 1;
        print VSCT "<VisibilityConstraints>\n";
    }
    elsif (/^\s*VISIBILITY_END/)
    {
        $bVisibilities = 0;
        print VSCT "</VisibilityConstraints>\n";
    }
    elsif (/^\s*KEYBINDINGS_SECTION/)
    {
        $bKeyBindings = 1;
        print VSCT "<KeyBindings>\n";
    }
    elsif (/^\s*KEYBINDINGS_END/)
    {
        $bKeyBindings = 0;
        print VSCT "</KeyBindings>\n";
    }
    elsif (/^\s*CMDUSED_SECTION/)
    {
        $bUsed = 1;
        print VSCT "<UsedCommands>\n";
    }
    elsif (/^\s*CMDUSED_END/)
    {
        $bUsed = 0;
        print VSCT "</UsedCommands>\n";
    }
}
