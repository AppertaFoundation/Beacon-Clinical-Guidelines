//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ClinicalGuidelines.Helpers
{
    public static class ImageHelper
    {
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                var imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            return Image.FromStream(ms, true);
        }

        public static Image ResizeImage(Image original, Size size)
        {
            var resized = new Bitmap(original, size.Width, original.Height * size.Width / original.Width);

            if (resized.Height > size.Height)
            {
                var cropArea = new Rectangle(0, (resized.Height - size.Height) / 2, size.Width, size.Height);
                resized = resized.Clone(cropArea, resized.PixelFormat);
            }

            return resized;
        }
    }
}