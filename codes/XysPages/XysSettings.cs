using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysSettings : WebBase
    {
        public override string InitialViewHtml()
        {
            string ViewHtml = string.Empty;

            Label Title = new Label();
            Title.Wrap.InnerText = Translator.Format("settings");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "28px");
            Title.Wrap.SetStyle(HtmlStyles.fontWeight, "bold");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "10px");

            Label mnu = new Label();
            mnu.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setroles");
            mnu.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("PartialView", "m=" + References.Pages.XysRole));
            mnu.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
            mnu.IDTag = "T100";

            Label mnu1 = new Label();
            mnu1.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setaccounts");
            mnu1.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("PartialView", "m=" + References.Pages.XysUser));
            mnu1.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
            mnu1.IDTag = "T110";

            Label mnu2 = new Label();
            mnu2.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setpages");
            mnu2.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("PartialView", "m=" + References.Pages.XysPage));
            mnu2.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
            mnu2.IDTag = "T120";

            Label mnu3 = new Label();
            mnu3.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setmenu");
            mnu3.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("PartialView", "m=" + References.Pages.XysMenu));
            mnu3.Wrap.SetAttribute(HtmlAttributes.@class, "mnulabel");
            mnu3.IDTag = "T130";

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.ClearStyles();
            elmBox.Wrap.SetStyle(HtmlStyles.marginLeft, "50px");
            elmBox.Wrap.SetStyle(HtmlStyles.marginTop, "60px");

            elmBox.AddItem(Title, 50);

            // if (IsMethodTag(mnu.IDTag)) elmBox.AddItem(mnu, 28);
            // if (IsMethodTag(mnu1.IDTag)) elmBox.AddItem(mnu1, 28);
            // if (IsMethodTag(mnu2.IDTag)) elmBox.AddItem(mnu2, 28);
            // if (IsMethodTag(mnu3.IDTag)) elmBox.AddItem(mnu3, 28);

            ViewHtml = elmBox.HtmlText;
            return ViewHtml;
        }
    }

}
