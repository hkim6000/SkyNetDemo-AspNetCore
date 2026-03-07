using SkyNet;
using SkyNet.ToolKit;
using System.Data;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysBases
{

    public class WebSingle : WebPage
    {
        protected AppKey AppKey = null;

        public WebSingle()
        {
            HtmlTranslator.Add(GetPageDict(this.GetType().Name));
        }

        protected List<Translator.DictionaryEntry> GetPageDict(string pagename)
        {
            List<Translator.DictionaryEntry> rlt = new();
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
            // Note: C# requires 'out' or 'ref' if the parameter is modified inside SQLDataTable
            DataTable dt = SQLData.SQLDataTable(SSQL,ref emsg);

            if (emsg == string.Empty && dt != null && dt.Rows.Count != 0)
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

        #region Simple Dialogs

        public string DialogMsgRequred(string msg_required = "msg_required")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_required));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            return dialogBox.HtmlText;
        }

        public string DialogMsgSaved(string _params, string Funcs = "PartialView", string msg_saved = "msg_saved")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_saved));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            if (_params == string.Empty)
            {
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:$PopOff();");
            }
            else
            {
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall(Funcs, _params) + "&$PopOff();");
            }
            return dialogBox.HtmlText;
        }

        public string DialogQstDelete(string Funcs, string msg_delete = "msg_delete")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_delete));
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
            dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall(Funcs));
            dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
            return dialogBox.HtmlText;
        }

        public string DialogMsgDeleted(string _params, string Funcs = "PartialView", string msg_deleted = "msg_deleted")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_deleted));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall(Funcs, _params) + "&$PopOff();");
            return dialogBox.HtmlText;
        }

        public string DialogMsg(string msg)
        {
            DialogBox dialogBox = new DialogBox(msg);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            return dialogBox.HtmlText;
        }

        #endregion
    }
}
