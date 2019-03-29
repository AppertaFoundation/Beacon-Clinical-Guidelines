//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using ClinicalGuidelines.DTOs;
using ClinicalGuidelines.Helpers;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Controllers
{
    [Authorize]
    [RoutePrefix("api/file")]
    public class FileApiController : ApiController
    {
        private const string UploadPathLocation = "~/Uploads/";
        private const int UploadFileSizeLimit = 10;
        private const int PixelMin = 100;
        private const int PixelMax = 1000;

        private readonly string[] _allowedTypes = { "image/jpeg", "image/png" };

        private string GetUploadPath()
        {
            return HostingEnvironment.MapPath(UploadPathLocation);
        }

        private bool IsAllowedType(string mimeType)
        {
            return _allowedTypes.Contains(mimeType);
        }

        private bool IsAllowedSize(int bytes)
        {
            return (ConvertBytesToMegaBytes(bytes) <= UploadFileSizeLimit);
        }

        private double ConvertBytesToMegaBytes(int bytes)
        {
            return Math.Round(bytes / 1024f / 1024f, 2);
        }

        [Route("upload")]
        public IHttpActionResult PostFile()
        {
            var uploadPath = GetUploadPath();
            if (uploadPath == null) return BadRequest("Uploads location unavailable");

            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count == 1)
                {
                    var postedFile = httpRequest.Files[0];

                    if (!IsAllowedSize(postedFile.ContentLength)) return BadRequest("File to large - Maximum allowed " + UploadFileSizeLimit + "mb");
                    if (!IsAllowedType(postedFile.ContentType)) return BadRequest("Incorrect file type");

                    var fileGuid = Guid.NewGuid();
                    var extension = Path.GetExtension(postedFile.FileName).ToLower();
                    var newFileName = fileGuid + extension;
                    var filePath = Path.GetFullPath(Path.Combine(uploadPath, newFileName));

                    postedFile.SaveAs(filePath);

                    var newFile = new FileDto()
                    {
                        Id = fileGuid,
                        Type = FileType.Image,
                        Host = HttpContext.Current.Request.Url.Authority,
                        Extension = extension,
                        UploadedBy = User.GetUsernameWithoutDomain(),
                        OriginalFileName = postedFile.FileName,
                        OriginalFileSize = ConvertBytesToMegaBytes(postedFile.ContentLength)
                    };

                    return Ok(newFile);
                }
                else
                {
                    return BadRequest("No Files Sent");
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("download/{imageFileName}")]
        public IHttpActionResult GetImage(string imageFileName)
        {
            var uploadPath = GetUploadPath();
            if (uploadPath == null) return BadRequest("Uploads location unavailable");

            var image = Image.FromFile(Path.GetFullPath(Path.Combine(uploadPath, imageFileName)));

            return Ok(ImageHelper.ImageToBase64(image, image.RawFormat));
        }

        [AllowAnonymous]
        [Route("download/{imageFileName}/thumb/width/{width:int}")]
        public IHttpActionResult GetImageAsThumbWithWidth(string imageFileName, int width)
        {
            if (width < PixelMin || width > PixelMax) return BadRequest("Requested crop out of bounds");

            var uploadPath = GetUploadPath();
            if (uploadPath == null) return BadRequest("Uploads location unavailable");

            var image = Image.FromFile(Path.GetFullPath(Path.Combine(uploadPath, imageFileName)));

            var height = (width * image.Height) / image.Width;

            return Ok(ImageHelper.ImageToBase64(ImageHelper.ResizeImage(image, new Size(width, height)), image.RawFormat));
        }

        [AllowAnonymous]
        [Route("download/{imageFileName}/thumb/height/{height:int}")]
        public IHttpActionResult GetImageAsThumbWithHeight(string imageFileName, int height)
        {
            if (height < PixelMin || height > PixelMax) return BadRequest("Requested crop out of bounds");

            var uploadPath = GetUploadPath();
            if (uploadPath == null) return BadRequest("Uploads location unavailable");

            var image = Image.FromFile(Path.GetFullPath(Path.Combine(uploadPath, imageFileName)));

            var width = (height * image.Width) / image.Height;

            return Ok(ImageHelper.ImageToBase64(ImageHelper.ResizeImage(image, new Size(width, height)), image.RawFormat));
        }

        [AllowAnonymous]
        [Route("download/{imageFileName}/thumb/{width:int}/{height:int}")]
        public IHttpActionResult GetImageAsThumbCropped(string imageFileName, int width, int height)
        {
            if (width < PixelMin || width > PixelMax) return BadRequest("Requested crop out of bounds");

            var uploadPath = GetUploadPath();
            if (uploadPath == null) return BadRequest("Uploads location unavailable");

            var image = Image.FromFile(Path.GetFullPath(Path.Combine(uploadPath, imageFileName)));

            var resizedImage = ImageHelper.ResizeImage(image, new Size(width, height));        

            return Ok(ImageHelper.ImageToBase64(resizedImage, image.RawFormat));
        }

        [AllowAnonymous]
        [Route("download/{imageFileName}/article-sized/{width:int}/{height:int}")]
        public IHttpActionResult GetImageArticleSized(string imageFileName, int width, int height)
        {
            if (width < PixelMin || width > PixelMax) return BadRequest("Requested crop out of bounds");

            var uploadPath = GetUploadPath();
            if (uploadPath == null) return BadRequest("Uploads location unavailable");

            var image = Image.FromFile(Path.GetFullPath(Path.Combine(uploadPath, imageFileName)));

            var resizedImage = ImageHelper.ResizeImage(image, new Size(width, height));        

            return Ok(ImageHelper.ImageToBase64(resizedImage, image.RawFormat));
        }

        
    }
}
