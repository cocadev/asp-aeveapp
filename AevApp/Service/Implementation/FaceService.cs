using System;
using System.IO;
using System.Threading.Tasks;
using AevApp.Helper.Interface;
using AevApp.Model;
using AevApp.Service.Interface;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Xamarin.SDK.Helper.Interface;

namespace AevApp.Service.Implementation
{
    public class FaceService : IFaceService
    {
        private readonly string _azureFaceServiceSubscriptionKey;
        private readonly string _azureFaceServiceApiRoot;

        public FaceService(IAppSettings appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            _azureFaceServiceSubscriptionKey = appSettings.Get(Constants.AzureFaceServiceSubscriptionKey);
            _azureFaceServiceApiRoot = appSettings.Get(Constants.AzureFaceServiceApiRoot);
        }

        public async Task<AevFace> DetectFaceAsync(MemoryStream image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            using (var faceServiceClient = FaceServiceClient())
            {
                var face = await faceServiceClient.DetectAsync(image);
                return new AevFace { face = face };
            }
        }

        public async Task<VerifyResult> VerifyFaceAsync(Guid face1Id, Guid face2Id)
        {
            using (var faceServiceClient = FaceServiceClient())
            {
                return await faceServiceClient.VerifyAsync(face1Id, face2Id);
            }
        }

        private FaceServiceClient FaceServiceClient()
        {
            return new FaceServiceClient(_azureFaceServiceSubscriptionKey, _azureFaceServiceApiRoot);
        }
    }
}