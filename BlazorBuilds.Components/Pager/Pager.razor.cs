using BlazorBuilds.Components.Common.Extensions;
using BlazorBuilds.Components.Common.Seeds;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorBuilds.Components.Pager;

public partial class Pager
{
    [Parameter, EditorRequired] public string AriaPagerLabel { get; set; } = default!;
    [Parameter] public string? AriaEllipseLabel              { get; set; } = default;
    [Parameter] public string? AriaInputDescription          { get; set; } = default;
    [Parameter] public string? NextLabel                     { get; set; } = default;
    [Parameter] public string? PreviousLabel                 { get; set; } = default;
    [Parameter] public string? InputLabel                    { get; set; } = default;
    [Parameter] public bool    ShowInputBox                  { get; set; } = false;
    [Parameter] public int     TotalItemCount                { get; set; } = 0;
    [Parameter] public int     ItemCount                     { get; set; } = 0;
    [Parameter] public int     ItemsPerPage                  { get; set; } = 0;
    [Parameter] public int     CurrentPage                   { get; set; } = 0;
    [Parameter] public string? PageCountFormatString         { get; set; } = default;
    [Parameter] public string? FilterCountFormatString       { get; set; } = default;
    [Parameter] public string? NoRecordsLabel                { get; set; } = default;

    [Parameter] public PagerInfoDisplay DisplayInfo          { get; set; } = PagerInfoDisplay.WithAnnouncement;
    [Parameter] public PagerDisplaySize DisplaySize          { get; set; } = PagerDisplaySize.Medium;
    [Parameter] public PagerButtonStyle ButtonStyle          { get; set; } = PagerButtonStyle.Round;
    [Parameter] public PagerNavType     NavigationType       { get; set; } = PagerNavType.Button;
    [Parameter] public StyleAs          StyleAs              { get; set; } = StyleAs.Dynamic;
    [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }

    [Inject]    private NavigationManager NavigationManager  { get; set; } = default!;

    private ElementReference? CurrentPageRef { get; set;}

    private string _nextLabel            = GlobalValues.Pager_Next_Label;
    private string _previousLabel        = GlobalValues.Pager_Previous_Label;
    private string _ariaEllipseLabel     = GlobalValues.Pager_Aria_Ellipse_Label;
    private string _ariaInputDescription = GlobalValues.Pager_Input_Description;
    private string _inputLabel           = GlobalValues.Pager_Input_Label;
    private string _pageCounterFormat    = GlobalValues.Pager_Count_Format_String;
    private string _filterCountFormat    = GlobalValues.Pager_Filter_Count_String;
    private string _noRecordsLabel       = GlobalValues.Pager_No_Records_String;

    private readonly string _liveRegionID = Guid.NewGuid().ToString();//Can have multiple pager components on the same page so need to differentiate between 
    private readonly string _inputID      = Guid.NewGuid().ToString();//live regions and the label to textbox id. 
    
    private string _pagerInformation = String.Empty; 

    private int _currentPage    = 0;
    private int _lastPage       = 0;
    private int _pageSlots      = 7;
    private int _inputNumber    = 1;
        
    protected override void OnInitialized()
    {
        if (String.IsNullOrWhiteSpace(AriaPagerLabel)) throw new ArgumentNullException(nameof(AriaPagerLabel), GlobalValues.Aria_Pager_Label_Exception_Message);

        _previousLabel        = String.IsNullOrWhiteSpace(PreviousLabel)           ? _previousLabel        : PreviousLabel;
        _nextLabel            = String.IsNullOrWhiteSpace(NextLabel)               ? _nextLabel            : NextLabel;
        _ariaEllipseLabel     = String.IsNullOrWhiteSpace(AriaEllipseLabel)        ? _ariaEllipseLabel     : AriaEllipseLabel;
        _ariaInputDescription = String.IsNullOrWhiteSpace(AriaInputDescription)    ? _ariaInputDescription : AriaInputDescription;
        _inputLabel           = String.IsNullOrWhiteSpace(InputLabel)              ? _inputLabel           : InputLabel;
        _pageCounterFormat    = String.IsNullOrWhiteSpace(PageCountFormatString)   ? _pageCounterFormat    : PageCountFormatString;
        _filterCountFormat    = String.IsNullOrWhiteSpace(FilterCountFormatString) ? _filterCountFormat    : FilterCountFormatString;
        _noRecordsLabel       = String.IsNullOrWhiteSpace(NoRecordsLabel)          ? _noRecordsLabel       : NoRecordsLabel;

    }
    protected override void OnParametersSet()
    {
        _lastPage         = (ItemCount < 1 || ItemsPerPage < 1) ? 0 : (int)Math.Ceiling((double)ItemCount / ItemsPerPage);
        _pageSlots        = Math.Min((int)DisplaySize, _lastPage);
        _currentPage      = _inputNumber  = CurrentPage;//Changed my mind about this (again) and put them back in sync, differs to video demo
        _pagerInformation = SetPagerInformation(_noRecordsLabel, _pageCounterFormat, _filterCountFormat, _currentPage, ItemsPerPage, ItemCount, TotalItemCount);
    }

    private static string SetPagerInformation(string noRecordsLabel, string pageCountFormatString, string filterCountFormatString, int currentPage, int itemsPerPage, int itemCount, int totalItemCount)
    {
        string filterString = String.Empty;

        if (itemCount <= 0) return noRecordsLabel;

        if (totalItemCount > itemCount) filterString = String.Format(filterCountFormatString, totalItemCount);

        int itemStart = (currentPage * itemsPerPage) - itemsPerPage + 1;
        int itemEnd = (currentPage * itemsPerPage) >= itemCount ? itemCount : (currentPage * itemsPerPage);

        if (itemCount < itemsPerPage) return String.Concat(String.Format(pageCountFormatString, currentPage, currentPage, itemCount, itemCount), " ", filterString);

        return String.Concat(String.Format(pageCountFormatString, currentPage, itemStart, itemEnd, itemCount), " ", filterString);
    }

    private string UpdateUriQueryParams(int pageNo, string pagePartName = "page")

        => NavigationManager.GetUriWithQueryParameter(pagePartName, pageNo);

    private static int GetPageNumber(int pageSlots, int slotNumber, int currentPage, int lastPage)
    {
        int pageNumber   = slotNumber;
        int centreSlots  = pageSlots - 4;
        int centreSlot   = (pageSlots / 2) + 1;

        int offsetFromCentre = slotNumber - centreSlot;
        int pagesLeft        = lastPage - currentPage;

        if (pageSlots >= lastPage) return pageNumber;

        if (slotNumber == 1) return pageNumber;

        if (slotNumber == pageSlots) return lastPage;

        if (slotNumber == 2 && currentPage > centreSlot) return -1;

        if (slotNumber > 2 && slotNumber < pageSlots -1 && currentPage > centreSlots)
        {
            int slotsToEnd = pageSlots - slotNumber;

            if (pagesLeft < 4) return lastPage - slotsToEnd;

            return currentPage + offsetFromCentre;
        }

        if (slotNumber == pageSlots -1) return pagesLeft < 4 ? lastPage -1 : -2;

        return pageNumber;
    }

    private async Task RequestedPage(int currentPage, int pageNumber, int lastPage, bool moveFocus = true)
    {
        if (pageNumber > 0 && pageNumber <= lastPage && pageNumber != currentPage)
        {
            _currentPage = pageNumber;
            
            (NavigationType == PagerNavType.Link).WhenTrue(() => NavigationManager.NavigateTo(UpdateUriQueryParams(pageNumber)));

            await CurrentPageChanged.HasDelegate.WhenTrue(() => CurrentPageChanged.InvokeAsync(pageNumber));

            await Task.Yield();//release control to render as we need the updated current page ref

            if (false == moveFocus) return;//called from input/next/previous items; we keep focus on them 
            
            await (true == moveFocus && CurrentPageRef is not null).WhenTrue(() => CurrentPageRef!.Value.FocusAsync());//ElementRef is null without numbered nav items

        }
    }

    private async Task HandleInputKeyDown(KeyboardEventArgs keyArgs, int currentPage, int lastPage)
    {
        if (keyArgs.Key == GlobalValues.KeyBoard_Enter_Key)
        { 
            _inputNumber = _inputNumber < 1 ? 1 : (_inputNumber > lastPage ? lastPage : _inputNumber);
            await RequestedPage(currentPage, _inputNumber, lastPage,false);
        }
    }

    private static string GetButtonClasses(PagerButtonStyle buttonStyle)

        => buttonStyle == PagerButtonStyle.Round ? $"{GlobalValues.Pager_Button_Class} {GlobalValues.Pager_Button_Round_Class}"
                                                 : $"{GlobalValues.Pager_Button_Class} {GlobalValues.Pager_Button_Square_Class}";

}
