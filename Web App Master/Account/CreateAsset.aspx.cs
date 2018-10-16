using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web_App_Master.Account
{
    public partial class CreateAsset : System.Web.UI.Page
    {
        protected void ShowError(string message, string title="Error")
        {
            CErrorBox.Visible = true;
            CErrorLabel.Text = title;
            CErrorMessage.Text = message;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var asset = new Asset();
                asset.Images = "";
                asset.AssetName = "none";
                Session["CreatorAsset"] = asset;
            }
        }        
        protected void CreateAssetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                foreach(var a in AssetController.GetAllAssets())
                {
                    if (AssetNumber.Text==a.AssetNumber)
                    {
                        ShowError("Please use an unique [Asset Number]");
                        return;
                    }
                }

                var asset = Session["CreatorAsset"] as Asset;
                if (asset == null) asset = new Asset();
                asset.AssetName = AssetName.Text;
                asset.AssetNumber = AssetNumber.Text;
                try
                {
                    asset.weight = Convert.ToDecimal(Weight.Text);
                }
                catch { asset.weight = 1; }
                if (CalibratedCheckBox.Checked)
                {
                    asset.IsCalibrated = true;
                    asset.CalibrationCompany = CalCompanyText.Text;
                    asset.CalibrationPeriod = CalPeriodText.Text;
                }
                asset.Description = DescriptionText.Text;
                var r = Push.Asset(asset);
            }
            catch
            {
                ShowError("Problem creating new asset");
            }
        }

        protected void UploadImg_Click(object sender, EventArgs e)
        {
            try
            {
                if(AssetNumber.Text=="")
                {
                    ShowError("You Must Enter Asset Number First");
                    return;
                }
                if (Global.AssetCache.Find((x)=>x.AssetNumber==AssetNumber.Text)!=null)
                {
                    ShowError("You Must Enter Unique Asset Number");
                    return;
                }


                if (creatorImageUploader.PostedFile!=null)
                {

                
               var asset = Session["CreatorAsset"] as Asset;

                    if (asset == null) asset = new Asset();
                    asset.AssetName = AssetName.Text;
                    asset.AssetNumber = AssetNumber.Text;
                    try
                    {
                        asset.weight = Convert.ToDecimal(Weight.Text);
                    }
                    catch { asset.weight = 1; }
                    if (CalibratedCheckBox.Checked)
                    {
                        asset.IsCalibrated = true;
                        asset.CalibrationCompany = CalCompanyText.Text;
                        asset.CalibrationPeriod = CalPeriodText.Text;
                    }
                    asset.Description = DescriptionText.Text;
                    if (!Directory.Exists(Server.MapPath("/Account/Images/" + asset.AssetNumber)))
                    {
                        Directory.CreateDirectory(Server.MapPath("/Account/Images/" + asset.AssetNumber));
                    }
                    var filename = Guid.NewGuid().ToString();
                var ext = Path.GetExtension(creatorImageUploader.FileName);
                creatorImageUploader.SaveAs(Server.MapPath("/Account/Images/" + asset.AssetNumber +"/"+ filename+ext));
                AssetImgBox.ImageUrl = "/Account/Images/" + asset.AssetNumber + "/" + filename + ext;
                asset.Images += filename + ext + ",";
                    Session["CreatorAsset"] = asset;
                //ImagePlaceHolder.Visible = true;
                }
            }
            catch
            {
                ShowError("Problem uploading image");
            }
        }
    }
}


