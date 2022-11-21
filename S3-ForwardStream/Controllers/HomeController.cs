using System.Diagnostics;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using S3_ForwardStream.Models;

namespace S3_ForwardStream.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //amazon setup
        private static AmazonS3Config config = new AmazonS3Config
        {
            AuthenticationRegion = RegionEndpoint.USEast1.SystemName,
            ForcePathStyle = true,
            UseHttp = true,
            ServiceURL = "http://s3.minio:9000/",
            SignatureVersion = "4"
        };
        private static AmazonS3Client s3Client = new AmazonS3Client("minio", "minio123", config);

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            // uncomment the following line if you like to troubleshoot communication with S3 storage and implement private void OnAmazonS3Exception(object sender, Amazon.Runtime.ExceptionEventArgs e)
            s3Client.ExceptionEvent += OnAmazonS3Exception;
        }

        private void OnAmazonS3Exception(object sender, ExceptionEventArgs e)
        {
            Debug.WriteLine(e.ToString());
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<ActionResult> DownloadLocalFileAsync()
        {
            ////amazon setup
            //BasicAWSCredentials basicCredentials = new BasicAWSCredentials("key", "secret");
            //AmazonS3Client s3Client = new AmazonS3Client(new BasicAWSCredentials("key", "secret"), Amazon.RegionEndpoint.USEast2);

            //GetObjectRequest getObjRequest = new GetObjectRequest()
            //{
            //    BucketName = "asd",
            //    Key = "a"
            //};

            //using var getObjResponseAsync = s3Client.GetObjectAsync(
            //    getObjRequest,
            //    new CancellationToken(false)
            //);
            var filename = "Get_Started_With_Smallpdf.pdf";
            var filename2 = "hugefile.pdf";

            var path = $"{System.IO.Directory.GetCurrentDirectory()}/temp/{filename2}";
            var fileStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);


            return new FileStreamResult(fileStream, contentType: "application/pdf")
            {
                FileDownloadName = "sample.pdf"
            };
        }

        public async Task<ActionResult> DownloadAWSFileAsync()
        {
            var filename = "Get_Started_With_Smallpdf.pdf";
            var filename2 = "hugefile.pdf";

            GetObjectRequest getObjRequest = new GetObjectRequest()
            {
                BucketName = "docs",
                Key = filename2
            };

            var getObjResponseAsync = s3Client.GetObjectAsync(
                getObjRequest,
                new CancellationToken(false)
            );


            return new FileStreamResult(getObjResponseAsync.Result.ResponseStream, contentType: "application/pdf")
            {
                FileDownloadName = "sample.pdf"
            };
        }

        public ActionResult DownloadLocalFileSync()
        {

            var filename = "Get_Started_With_Smallpdf.pdf";
            var filename2 = "hugefile.pdf";

            var path = $"{System.IO.Directory.GetCurrentDirectory()}/temp/{filename2}";
            var fileStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);


            return new FileStreamResult(fileStream, contentType: "application/pdf")
            {
                FileDownloadName = "sample.pdf"
            };
        }

        public ActionResult DownloadAWSFileSync()
        {
            var filename = "Get_Started_With_Smallpdf.pdf";
            var filename2 = "hugefile.pdf";

            GetObjectRequest getObjRequest = new GetObjectRequest()
            {
                BucketName = "docs",
                Key = filename2
            };

            var getObjResponseAsync = s3Client.GetObjectAsync(
                getObjRequest,
                new CancellationToken(false)
            );

            return new FileStreamResult(getObjResponseAsync.Result.ResponseStream, contentType: "application/pdf")
            {
                FileDownloadName = "sample.pdf"
            };
        }

        public ActionResult DownloadAWSFileSync2()
        {
            var filename2 = "hugefile2.pdf";

            GetObjectRequest getObjRequest = new GetObjectRequest()
            {
                BucketName = "docs",
                Key = filename2
            };

            var getObjResponseAsync = s3Client.GetObjectAsync(
                getObjRequest,
                new CancellationToken(false)
            );

            return new FileStreamResult(getObjResponseAsync.Result.ResponseStream, contentType: "application/pdf")
            {
                FileDownloadName = "sample2.pdf"
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}