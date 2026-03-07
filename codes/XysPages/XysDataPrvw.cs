using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysDataPrvw : WebPage
    {
        private XData xData = null;

        public XysDataPrvw()
        {
            HtmlTranslator.Add(GetPageDict(this.GetType().Name));

            string dataId = DecryptString(QueryValue("x"));
            string ssql = " select DataId, DataName, DataDesc,DataModel from XysData where DataId = N'" + dataId + "' ";

            DataTable dt = SQLData.SQLDataTable(ssql);

            if (dt != null && dt.Rows.Count > 0)
            {
                xData = new XData
                {
                    DataId = dt.Rows[0][0].ToString(),
                    DataName = dt.Rows[0][1].ToString(),
                    DataDesc = dt.Rows[0][2].ToString(),
                    DataElements = (List<XDataElement>)DeserializeObject(dt.Rows[0][3].ToString(), typeof(List<XDataElement>))
                };
            }
        }

        protected internal List<Translator.DictionaryEntry> GetPageDict(string pagename)
        {
            List<Translator.DictionaryEntry> rlt = new List<Translator.DictionaryEntry>();
            string SSQL = " declare @pageid nvarchar(50),@isocode nvarchar(10)  " +
                          " set @pageid = N'" + pagename + "' " +
                          " set @isocode = N'" + ClientLanguage + "' ";

            switch (ClientLanguage.Contains("-"))
            {
                case true:
                    SSQL += " if exists(select * from XysDict where Isocode =  @isocode) " +
                           " begin " +
                           "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                           "  Where (Target = @pageid or Target = '*') " +
                           "          and (Isocode = '*' or Isocode = @isocode ) " +
                           "  order by KeyWord  " +
                           " end " +
                           " else " +
                           " begin " +
                           "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                           "  Where (Target = @pageid or Target = '*') " +
                           "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                           "  order by KeyWord  " +
                           " end ";
                    break;
                case false:
                    SSQL += " if exists(select * from XysDict where left(Isocode,2) =  @isocode) " +
                           " begin " +
                           "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                           "  Where (Target = @pageid or Target = '*') " +
                           "          and (Isocode = '*' or Isocode = @isocode ) " +
                           "  order by KeyWord  " +
                           " end " +
                           " else " +
                           " begin " +
                           "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                           "  Where (Target = @pageid or Target = '*') " +
                           "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                           "  order by KeyWord  " +
                           " end ";
                    break;
            }

            string emsg = string.Empty;
            DataTable dt = SQLData.SQLDataTable(SSQL, ref emsg);
            if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Translator.DictionaryEntry _Dict = new Translator.DictionaryEntry();
                    _Dict.IsoCode = dt.Rows[i][1].ToString();
                    _Dict.DicKey = dt.Rows[i][2].ToString();
                    _Dict.DicWord = dt.Rows[i][3].ToString();
                    rlt.Add(_Dict);
                }
            }
            return rlt;
        }

        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("preview") + " - " + xData.DataName);

            Label labelT = new Label();
            labelT.Wrap.SetStyles("font-weight:700;font-size:22px; margin-left:10px; ");
            labelT.Wrap.InnerText = xData.DataName;

            Label labelT1 = new Label();
            labelT1.Wrap.SetStyles("font-size:14px;text-decoration:italic; margin-left:12px; color:#666; ");
            labelT1.Wrap.InnerText = "- " + xData.DataDesc;

            HtmlElementBox elmBox1 = new HtmlElementBox();
            elmBox1.DefaultStyle = false;
            elmBox1.Wrap.SetStyle(HtmlStyles.boxShadow, "");
            elmBox1.SetStyle(HtmlStyles.width, "92%");
            elmBox1.SetStyle(HtmlStyles.margin, "auto");
            elmBox1.SetStyle(HtmlStyles.marginTop, "30px");

            elmBox1.AddItem(labelT, 6);
            elmBox1.AddItem(labelT1, 20);

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "92%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "10px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "30px");

            elmBox.AddItem(PreviewData());

            HtmlDoc.HtmlBodyAddOn = elmBox1.HtmlText + elmBox.HtmlText;
        }

        private UIControl PreviewData()
        {
            WebBase wb = new WebBase();
            UIControl _UIControl = wb.UIControlFromXDataElements(xData.DataName, xData.DataElements, UIModes.@New);
            return _UIControl;
        }
    }

}
