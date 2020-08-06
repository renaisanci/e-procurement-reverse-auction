using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Ecc.Web.Infrastructure.Core
{
    public class UploadMultipartFormProvider : MultipartFormDataStreamProvider
    {
        private string produtoId;
        private string imagemGrande;

        public UploadMultipartFormProvider(string rootPath, string _produtoId, string _imageGrande)
            : base(rootPath)
        {

            produtoId = _produtoId;
            imagemGrande = _imageGrande;
        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers != null &&
                headers.ContentDisposition != null)
            {
                int intCaracterIni = headers.ContentDisposition.FileName.TrimEnd('"')
                    .TrimStart('"')
                    .Length < 60 ? 0 : headers.ContentDisposition.FileName.Length - 60;
                
                int intCaracterFinal = headers.ContentDisposition.FileName.TrimEnd('"')
                    .TrimStart('"')
                    .Length - intCaracterIni;

                Random randNum = new Random();
                var random = randNum.Next();

                var arrayImage = headers.ContentDisposition.FileName.TrimStart('"').TrimEnd('"').Split('.');
                var ultimoArrayImage = arrayImage.Length - 1;
                var extensaoImage = arrayImage[ultimoArrayImage];

                headers.ContentDisposition.FileName = produtoId + "_" + random+"." + extensaoImage;

                return headers.ContentDisposition.FileName;
            }

            return base.GetLocalFileName(headers);
        }
    }
}