using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysFormPrvw: WebPage
    {

        XForm xForm = null;

        public XysFormPrvw()
        {
            HtmlTranslator.Add(GetPageDict(this.GetType().Name));

            string formId = DecryptString(QueryValue("x"));
            string ssql = " select FormModel from XysForm where FormId = N'" + formId + "' ";

            string xFormString = SQLData.SQLFieldValue(ssql);
            xForm = (XForm)DeserializeObject(xFormString, typeof(XForm));
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
            HtmlDoc.SetTitle(Translator.Format("preview") + " - " + xForm.Title);

            HtmlDoc.HtmlBodyAddOn = xForm == null ? string.Empty : PreviewData().HtmlText;
        }

        private UIForm PreviewData()
        {
            WebBase wb = new WebBase();
            UIForm _UIForm = wb.UIFormFromXForm(xForm, UIModes.@New);
            return _UIForm;
        }

    }
}
