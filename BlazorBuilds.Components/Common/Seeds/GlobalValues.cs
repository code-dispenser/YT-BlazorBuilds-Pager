namespace BlazorBuilds.Components.Common.Seeds;

internal class GlobalValues
{
    public const string Aria_Pager_Label_Exception_Message = "The aria pager label is required. It cannot be null, empty or whitespace";

    public const string KeyBoard_Enter_Key         = "Enter";

    public const string Pager_Class                = "pager";
    public const string Pager_Label_Class          = $"{Pager_Class}__label";
    public const string Pager_Input_Class          = $"{Pager_Class}__input";
    public const string Pager_Information_Class    = $"{Pager_Class}__information";
    public const string Pager_Controls_Class       = $"{Pager_Class}__controls";
    public const string Pager_Counter_Class        = $"{Pager_Class}__counter";
    public const string Pager_Buttons_Class        = $"{Pager_Class}__buttons";
    public const string Pager_Button_Class         = $"{Pager_Class}__button";
    public const string Pager_Button_Round_Class   = $"{Pager_Button_Class}--round";
    public const string Pager_Button_Square_Class  = $"{Pager_Button_Class}--square";
    public const string Pager_Icon_Class           = $"{Pager_Class}__icon";
    
    public const string Pager_Next_Label           = "Next";
    public const string Pager_Previous_Label       = "Previous";
    public const string Pager_Aria_Ellipse_Label   = "Ellipse, indicating skipped pages";
    public const string Pager_Input_Label          = "Go to page:";
    public const string Pager_Input_Description    = "Enter key to activate.";
    public const string Pager_No_Records_String    = "No entries found.";
    public const string Pager_Count_Format_String  = "Page {0}, entries {1} to {2} of {3}";
    public const string Pager_Filter_Count_String  = "(filtered from {0} total entries)";
    public const string Pager_Next_Icon_Name_Class = "navigate-next-icon";
    public const string Pager_Prev_Icon_Name_Class = "navigate-previous-icon";


    public const string Style_As_Dark  = "dark";
    public const string Style_As_Light = "light";

    public static string? GetStyleAsValue(StyleAs styleAs)

    => styleAs switch
    {
        StyleAs.OnLight => Style_As_Light,
        StyleAs.OnDark => Style_As_Dark,
        _ => null
    };

}
