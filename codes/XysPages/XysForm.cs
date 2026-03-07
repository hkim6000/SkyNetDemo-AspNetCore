using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysForm : WebBase
    {
        public override string InitialViewHtml()
        {
            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
            HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

            // Initialize Grid filters
            HtmlDoc.InitialScripts.RemoveLocalValue("FilterBoxValue");
            HtmlDoc.InitialScripts.RemoveLocalValue("DataGrid_Filter");

            TitleSection2 PageLayout = PageTitle();
            PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
            PageLayout.ContentWrap.InnerText = PartialPage(References.Pages.XysFormMV);

            return PageLayout.HtmlText;
        }

        public ApiResponse PartialView()
        {
            string m = GetDataValue("m");
            string t = GetDataValue("t");

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(m, t));
            _ApiResponse.ExecuteScript("$ScrollToTop()");
            return _ApiResponse;
        }

        public ApiResponse Navigate()
        {
            string m = GetDataValue("m");
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(m);
            return _ApiResponse;
        }
    }

    public class XForm
    {
        public string FormId = string.Empty;
        public string FormRef = string.Empty;
        public int FormFlag = 1;
        public string Title = string.Empty;
        public int IsTitle = 1;
        public string Description = string.Empty;
        public string TitleAttributes = string.Empty;
        public string TitleStyles = string.Empty;
        public string WrapAttributes = string.Empty;
        public string WrapStyles = string.Empty;
        public int Paper = 1;
        public int Orientation = 0;
        public int MarginTop = 10;
        public int MarginLeft = 20;
        public int IsBorderLine = 0;
        public int IsAttached = 0;
        public List<XFormSection> Sections = new List<XFormSection>();
    }

    public class XFormSection
    {
        public string SectionId = string.Empty;
        public int Seq = 0;
        public string Name = string.Empty;
        public string Label = string.Empty;
        public string LabelAttributes = string.Empty;
        public string LabelStyles = string.Empty;
        public string WrapAttributes = string.Empty;
        public string WrapStyles = string.Empty;
        public int Columns = 3;
        public int FlexibleRows = 0;
        public int BorderLine = 1;
        public string RefId = string.Empty;
        public List<XFormElement> Elements = new List<XFormElement>();
    }

    public class XFormElement
    {
        public string ElementId = string.Empty;
        public int Seq = 0;
        public string Name = string.Empty;
        public string Label = string.Empty;
        public string Value = string.Empty;
        public int UIType = 17;
        public string Styles = string.Empty;
        public string Attributes = string.Empty;
        public string LabelStyles = string.Empty;
        public string LabelAttributes = string.Empty;
        public string WrapStyles = string.Empty;
        public string WrapAttributes = string.Empty;
        public int IsKey = 0;
        public int IsVisible = 1;
        public int IsReadOnly = 0;
        public int IsRequired = 0;
        public int IsFixedValue = 0;
        public int NewLineAfter = 0;
        public int RowSpan = 0;
        public int ValueType = 0;
        public string InitialValues = string.Empty;
        public string RefId = string.Empty;
    }

}
