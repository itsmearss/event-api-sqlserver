using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;
using TestProjectAnnur.Services;
using OfficeOpenXml;
using System.Net.Http.Headers;

namespace TestProjectAnnur.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnv;

        public UserController(IUserService userService, IWebHostEnvironment webHostEnv)
        {
            _userService = userService;
            _webHostEnv = webHostEnv;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var userEntity = await _userService.GetUserByIdAsync(id);

            if (userEntity == null)
                return NotFound($"User dengan ID {id} tidak ditemukan");

            return Ok(userEntity);
        }

        [HttpGet("getByUsername/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var userEntity = await _userService.GetUserByUsernameAsync(username);

            if (userEntity == null)
                return NotFound($"User dengan Username {username} tidak ditemukan");

            return Ok(userEntity);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUser = await _userService.CreateUserAsync(userDTO);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateUserAsync(id, userDTO);

            if (updatedUser == null)
                return NotFound($"User dengan ID {id} tidak ditemukan");

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deletedUser = await _userService.DeleteUserAsync(id);

            if (!deletedUser)
                return NotFound($"User dengan ID {id} tidak ditemukan");

            return Ok(deletedUser);
        }

        [HttpPost("upload-excel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadUser()
        {
            int baris = 0;
            bool finishedExcelRead = false;
            string rootPath = _webHostEnv.ContentRootPath;
            Console.WriteLine(rootPath);
            double oadatenum;
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var pathToSave = Path.Combine(rootPath, "Uploads");

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var kind = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    var fullPath = Path.Combine(pathToSave, "MM" + DateTime.Now.ToString("yyMMddHHmmss") + "_" + fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var excelFile = new FileInfo(fullPath);
                    var excelData = new List<UserDTO>();
                    using (var p = new ExcelPackage(excelFile))
                    {
                        var ws = p.Workbook.Worksheets.FirstOrDefault();
                        var endrow = ws.Dimension.End.Row;
                        for (int row = 2; row <= endrow; row++)
                        {
                            if (ws.Cells[row, 1].Value?.ToString()?.Trim() is null)
                                break;

                            baris = row;
                            var d = new UserDTO();
                            d.Username = ws.Cells[row, 2].Value?.ToString()?.Trim();
                            d.Fullname = ws.Cells[row, 3].Value?.ToString()?.Trim();
                            d.Password = ws.Cells[row, 4].Value?.ToString()?.Trim();
                            d.RoleId = int.Parse(ws.Cells[row, 5].Value?.ToString().Trim());
                            excelData.Add(d);
                        }
                        finishedExcelRead = true;
                    }
                    var response = await _userService.ProcessUserImport(excelData);
                    if (response.IsSuccess)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest(response.Message);
                    }
                }
                return BadRequest("Empty file!");
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var inex = ex.InnerException?.Message ?? "⚠";
                var errExcelRow = !finishedExcelRead ? " - Error on your excel file at row " + baris : "";
                return StatusCode(500, $"Internal server error: {message} {inex} {errExcelRow}");
            }
        }
    }
}
