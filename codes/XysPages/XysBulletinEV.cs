using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysBulletinEV : WebBase
    {
        public XysBulletinEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "BltnId", flag = true },
            new NameValueFlag { name = "BltnTitle" },
            new NameValueFlag { name = "BltnMemo" },
            new NameValueFlag { name = "CreatedBy" },
            new NameValueFlag { name = "Files", flag = true },
            new NameValueFlag { name = "FileRefId", flag = true }
            });
        }
        public override void InitialViewData()
        {
            string SSQL = " Select  BltnId, BltnTitle, BltnMemo, CreatedBy, dbo.XF_FileList(FileRefId) Files, FileRefId, SYSDTE, SYSUSR  " +
                          " from XysBulletin where BltnId = " + PartialData;

            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newnotice") : Translator.Format("editnotice");

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Texts text = new Texts(Translator.Format("title"), ViewPart.Field("BltnTitle").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "60%");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("BltnTitle").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
            text.Wrap.SetStyle(HtmlStyles.display, "block");

            TextArea text1 = new TextArea(Translator.Format("memo"));
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "90%");
            text1.Text.SetStyle(HtmlStyles.height, "200px");
            text1.Text.SetAttribute(HtmlAttributes.id, ViewPart.Field("BltnMemo").name);
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "600");
            text1.Text.InnerText = ViewPart.Field("BltnMemo").value;
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text2 = new Texts(Translator.Format("createdby"), ViewPart.Field("CreatedBy").name, TextTypes.text);
            text2.Required = true;
            text2.Text.SetStyle(HtmlStyles.width, "50%");
            text2.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Data == null ? AppKey.UserName : ViewPart.Field("CreatedBy").value);
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
            text2.Wrap.SetStyle(HtmlStyles.display, "block");

            FileUpload fileRef = new FileUpload();
            fileRef.File.SetAttribute(HtmlAttributes.id, ViewPart.Field("Files").name);
            fileRef.Wrap.SetAttribute(HtmlAttributes.@class, "__filebtn");
            fileRef.Button.SetStyles("padding: 8px; border-radius: 4px; border: 1px solid rgb(68, 68, 68); border-image: none; color: rgb(68, 68, 68); font-size: 12px; cursor: pointer; background-color: rgb(255, 255, 255);");
            fileRef.Label.SetStyle(HtmlStyles.paddingLeft, "10px");
            fileRef.Label.InnerText = ViewPart.Field("Files").value;

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

            elmBox.AddItem(text, 1);
            elmBox.AddItem(text1, 1);
            elmBox.AddItem(text2, 20);
            elmBox.AddItem(fileRef, 20);
            elmBox.AddItem(ViewButtons, 20);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;
            return ViewHtml;
        }

        public ApiResponse SaveData()
        {
            string BltnTitle = ViewPart.Field("BltnTitle").value;
            string BltnMemo = ViewPart.Field("BltnMemo").value;
            string CreatedBy = ViewPart.Field("CreatedBy").value;

            if (ViewPart.Data == null) ViewPart.Field("FileRefId").value = NewID(1);

            ApiResponse _ApiResponse = new ApiResponse();
            if (BltnTitle == string.Empty || BltnMemo == string.Empty || CreatedBy == string.Empty)
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (rlt == string.Empty)
                {
                    rlt = UploadFile(ViewPart.Field("Files").name, ViewPart.Field("FileRefId").value);
                    if (rlt == string.Empty)
                    {
                        _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysBulletinMV"), References.Elements.PageContents);
                    }
                    else
                    {
                        _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                    }
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                }
            }

            return _ApiResponse;
        }

        private string PutSaveData()
        {
            List<string> SQL = new List<string>();

            if (ViewPart.Data == null)
            {
                SQL.Add(" declare @seq int " +
                        " exec dbo.XP_NextSeq N'XysBulletin',N'BltnId',@seq out " +
                        " insert into XysBulletin(BltnId, BltnTitle, BltnMemo, CreatedBy, FileRefId, SYSDTE, SYSUSR) " +
                        " values(@seq,@BltnTitle,@BltnMemo,@CreatedBy,@FileRefId,getdate(),@SYSUSR)    ");
            }
            else
            {
                SQL.Add(" Update XysBulletin set " +
                        "   BltnTitle = @BltnTitle, BltnMemo = @BltnMemo, CreatedBy = @CreatedBy, " +
                        "   FileRefId = (case when @FileRefId='' then FileRefId else @FileRefId end), " +
                        "   SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " Where  BltnId = @BltnId  ");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@BltnId", Value = Convert.ToDouble(ViewPart.Field("BltnId").value).ToString(), SqlDbType = System.Data.SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@BltnTitle", Value = ViewPart.Field("BltnTitle").value, SqlDbType = System.Data.SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@BltnMemo", Value = ViewPart.Field("BltnMemo").value, SqlDbType = System.Data.SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@CreatedBy", Value = ViewPart.Field("CreatedBy").value, SqlDbType = System.Data.SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FileRefId", Value = ViewPart.Field("FileRefId").value, SqlDbType = System.Data.SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = System.Data.SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysBulletinEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (rlt == string.Empty)
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysBulletinMV"), References.Elements.PageContents);
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        private string PutDeleteData()
        {
            List<string> SQL = new List<string>
            {
                " delete from XysBulletin where BltnId = @BltnId "
            };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@BltnId", Value = Convert.ToDouble(ViewPart.Field("BltnId").value).ToString(), SqlDbType = System.Data.SqlDbType.Int });

            return PutData(SqlWithParams(SQL, SqlParams));
        }


    }
}
