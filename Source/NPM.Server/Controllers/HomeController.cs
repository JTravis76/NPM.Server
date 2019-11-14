using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace NPM.Server.Controllers
{
    /// <summary>
    /// This is the default root routing
    /// </summary>
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHostingEnvironment _hosting;
        private readonly IConfiguration _configuration;

        public HomeController(IHostingEnvironment hosting, IConfiguration configuration)
        {
            _hosting = hosting ?? throw new ArgumentNullException(nameof(hosting));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        [AllowAnonymous]
        [Route("{name}")]
        [HttpGet]
        public IActionResult PackageInfo_GET(string name)
        {
            string file = $@"{_hosting.ContentRootPath}\NPM-Packages\{name}\{name}.json";

            if (name != null && name.StartsWith("@") && name.Contains("%2f"))
            {
                string[] values = name.Split("%2f");
                file = $@"{_hosting.ContentRootPath}\NPM-Packages\{values[0]}\{values[1]}\{values[1]}.json";
            }
            
            if (System.IO.File.Exists(file))
            {
                string text = System.IO.File.ReadAllText(file);
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(text);
                return Ok(obj);
            }

            return BadRequest("Package doesn't exist");
        }

        [AllowAnonymous]
        [Route("{org}/{name}")]
        [HttpGet]
        public IActionResult OrgPackageInfo_GET(string org, string name)
        {
            string file = $@"{_hosting.ContentRootPath}\NPM-Packages\{org}\{name}\{name}.json";
            if (System.IO.File.Exists(file))
            {
                string text = System.IO.File.ReadAllText(file);
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(text);
                return Ok(obj);
            }

            return BadRequest("Package doesn't exist");
        }

        [AllowAnonymous]
        [Route("{name}/-/{package}")]
        [HttpGet]
        public IActionResult Package_GET(string name, string package)
        {
            byte[] file = null;

            using (System.IO.FileStream fs = System.IO.File.OpenRead($@"{_hosting.ContentRootPath}\NPM-Packages\{name}\{package}"))
            {
                //Read into bytes so I can dispose the stream
                file = new byte[fs.Length];
                fs.Read(file, 0, file.Length);
            }
            return File(file, "application/octet-stream", package);
        }

        [AllowAnonymous]
        [Route("{org}/{name}/-/{org1}/{package}")]
        [HttpGet]
        public IActionResult OrgPackage_GET(string org, string org1, string name, string package)
        {
            byte[] file = null;

            string filePath = $@"{_hosting.ContentRootPath}\NPM-Packages\{org}\{name}\{package}";

            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.FileStream fs = System.IO.File.OpenRead(filePath))
                {
                    //Read into bytes so I can dispose the stream
                    file = new byte[fs.Length];
                    fs.Read(file, 0, file.Length);
                }
                return File(file, "application/octet-stream", package);
            }

            return BadRequest($"Package {package} is not available");
        }

        [Authorize]
        [Route("{name}")]
        [HttpPut]
        public IActionResult Package_PUT(Newtonsoft.Json.Linq.JObject jObj)
        {
            string packageName = jObj.SelectToken("name").ToString();
            string version = jObj.SelectToken("dist-tags.latest").ToString();
            string encodedData = jObj.SelectToken("_attachments.*.data").ToString();
            string directory = $@"{_hosting.ContentRootPath}\NPM-Packages\{packageName}";

            //clear the attachment before saving JSON
            jObj.SelectToken("_attachments").Replace(Newtonsoft.Json.Linq.JObject.Parse("{}"));

            //Might have scope value; @ports/package
            if (packageName.StartsWith("@") && packageName.Contains("/"))
            {
                string[] values = packageName.Split("/");
                packageName = values[1];
                directory = $@"{_hosting.ContentRootPath}\NPM-Packages\{values[0]}\{packageName}";
            }
            
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);

                // Add Timestamps
                Newtonsoft.Json.Linq.JObject time = new Newtonsoft.Json.Linq.JObject
                {
                    { "created", DateTime.Now },
                    { "modified", DateTime.Now },
                    { version, DateTime.Now }
                };
                jObj.Add("time", time);

                System.IO.File.WriteAllText($@"{directory}\{packageName}.json", jObj.ToString());
            }
            else
            {
                // Add Timestamps for this version to be merged later
                jObj.Add("time", new Newtonsoft.Json.Linq.JObject { { version, DateTime.Now } });


                string jsonFile = $"{directory}\\{packageName}.json";
                if (System.IO.File.Exists(jsonFile))
                {
                    string fileContent = System.IO.File.ReadAllText(jsonFile);
                    var originalJson = Newtonsoft.Json.Linq.JObject.Parse(fileContent);
                    var versions = originalJson.SelectTokens("versions.*.version");

                    if (versions.ToList().Contains(version))
                    {
                        return StatusCode(400, new { ok = "package already exists", success = false });
                    }
                    else
                    {
                        originalJson.Merge(jObj, new Newtonsoft.Json.Linq.JsonMergeSettings
                        {
                            MergeArrayHandling = Newtonsoft.Json.Linq.MergeArrayHandling.Union
                        });

                        System.IO.File.WriteAllText($@"{directory}\{packageName}.json", originalJson.ToString());
                    }
                }
                else
                {
                    return StatusCode(500, "cannot find package json to merge");
                }
            }

            System.IO.File.WriteAllBytes($@"{directory}\{packageName}-{version}.tgz", Convert.FromBase64String(encodedData));            

            return StatusCode(201, new { ok = "created new package", success = true });
        }

        //[AllowAnonymous]
        //[Route("-/v1/login")]
        //[HttpPost]
        //public IActionResult Login_POST(dynamic model)
        //{
        //    model = "{"hostname":"PA123456"}";
        //    return Ok();
        //}

        [AllowAnonymous]
        [Route("-/user/org.couchdb.user:{user}")]
        [HttpPut]
        public IActionResult Login_PUT(BusinessObjects.Dtos.AddUser addUser)
        {
            List<Claim> claims = null;
            string token = "";

            claims = new List<Claim>
            {
                new Claim("AuthDate", addUser.date.ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, ""),
                new Claim(ClaimTypes.Name, addUser.name)//<- User.Identity.Name
                //new Claim("CustomClaim", "myCustomValue")
            };

            foreach (string role in addUser.roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            if (claims != null)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var jwt = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                        _configuration["Jwt:Issuer"],
                        claims,
                        expires: DateTime.Now.AddMonths(3),
                        signingCredentials: credentials);

                token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Ok(new { ok = $"user '{addUser.name}' created", token });
            }

            return BadRequest("could not create user");
        }

        [Authorize]
        [Route("-/whoami")]
        [HttpGet]
        public IActionResult Whoami_GET()
        {
            return Ok(new { username = User.Identity.Name });
        }


    }
}
