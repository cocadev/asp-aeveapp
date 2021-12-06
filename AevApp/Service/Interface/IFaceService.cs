using System;
using System.IO;
using System.Threading.Tasks;
using AevApp.Model;
using Microsoft.ProjectOxford.Face.Contract;

namespace AevApp.Service.Interface
{
    public interface IFaceService
    {
        Task<AevFace> DetectFaceAsync(MemoryStream image);
        Task<VerifyResult> VerifyFaceAsync(Guid face1Id, Guid face2Id);
    }
}