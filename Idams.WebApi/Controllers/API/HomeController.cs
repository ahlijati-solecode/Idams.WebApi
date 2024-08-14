using Idams.Core.Model.Dtos;
using Idams.Core.Services;
using Idams.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Idams.WebApi.Controllers.API
{
    [Route("Home")]
    [ApiController]
    public class HomeController : ApiController
    {
        private readonly IHierLevelService _iHierLevelService;
        private readonly IHomeService _homeService;
        public const string Success = "Success";

        public HomeController(IHierLevelService iHierLevelService, IHomeService homeService)
        {
            _iHierLevelService = iHierLevelService;
            _homeService = homeService;
        }

        [HttpGet("DownloadList")]
        public async Task<IActionResult> DownloadList()
        {
            var documents = new List<DocAboutIdamsDto>();
            for (int i = 0; i < 3; i++)
            {
                documents.Add(new DocAboutIdamsDto()
                {
                    Name = "Introduction IDAMS",
                    Cover = "",
                    FileLink = $"{EncryptUtils.Convert_StringvalueToHexvalue("dummy.pdf", System.Text.Encoding.Unicode)}"
                });
            }
            return Ok(documents);
        }

        [HttpGet("downloadfile/{filename}")]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            filename = EncryptUtils.Convert_HexvalueToStringvalue(filename, System.Text.Encoding.Unicode);
            string path = Path.Combine(Environment.CurrentDirectory, @"Asset/") + filename;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/pdf", filename);
        }

        [HttpGet("HierName")]
        public async Task<IActionResult> HierName()
        {
            try
            {
                var data = await _iHierLevelService.GetHierLvlLabel();
                return Ok(data);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(false);
            }
        }

        [HttpGet("AboutUrls")]
        public async Task<IActionResult> GetAboutUrls()
        {
            var result = await _homeService.GetAboutIdamsUrls();
            Message = Success;
            return Ok(result);
        }
    }
}
