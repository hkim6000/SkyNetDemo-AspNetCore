using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysData : WebBase
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
            PageLayout.ContentWrap.InnerText = PartialPage(References.Pages.XysDataMV);

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

    public class XData
    {
        public string DataId = string.Empty;
        public string DataName = string.Empty;
        public string DataDesc = string.Empty;
        public List<XDataElement> DataElements = new List<XDataElement>();
    }

    public class XDataElement
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
        public int IsKey = 0;
        public int IsVisible = 1;
        public int IsReadOnly = 0;
        public int IsRequired = 0;
        public int IsFixedValue = 0;
        public int LineSpacing = 0;
        public int ValueType = 0;
        public string InitialValues = string.Empty;
    }

}
