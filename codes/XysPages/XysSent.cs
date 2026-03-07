using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysSent : WebSingle
    {
        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("reset"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Label lbl1 = new Label(Translator.Format("sentemail"));
            lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
            lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

            Label lbl2 = new Label(Translator.Format("waitemail"));
            lbl2.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
            lbl2.Wrap.SetStyle(HtmlStyles.color, "#444");

            Button btn = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
            btn.SetStyle(HtmlStyles.marginLeft, "6px");
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");

            elmBox.AddItem(Title, 30);
            elmBox.AddItem(lbl1, 16);
            elmBox.AddItem(lbl2, 30);
            elmBox.AddItem(btn, 10);

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
        }

        public ApiResponse NavXysSignin()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysSignin);
            return _ApiResponse;
        }
    }

}
