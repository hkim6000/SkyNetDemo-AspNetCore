using SkyNet;
using SkyNet.ToolKit;

namespace ASPNETCoreWeb.codes.XysBases
{
    public class WebGrid : WebBase
    {
        protected string MyPageType = string.Empty;

        public override string InitialViewHtml()
        {
            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
            HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

            TitleSection2 PageLayout = PageTitle();
            PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);

            //Initialize Grid filters & get initial view
            HtmlDoc.InitialScripts.RemoveLocalValue("FilterBoxValue");
            HtmlDoc.InitialScripts.CallAction("InitialView", string.Empty);

            //Dim PageLayout As TitleSection2 = PageTitle()
            //PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents)
            //PageLayout.ContentWrap.InnerText = PartialPage(MyPageType + "MV")

            return PageLayout.HtmlText;
        }

        public ApiResponse InitialView()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(MyPageType + "MV"));
            return _ApiResponse;
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

}
