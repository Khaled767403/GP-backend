using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using otherServices.Data_Project.Models;
using otherServices.Models;
using otherServices.Models.DTOs;
using otherServices.Repositories;
using otherServices.Services;
using WebAPIDotNet.DTOs;

namespace WebAPIDotNet.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _env;


        public AuthService(IJwtService jwtService,IUserRepository userRepository,IWebHostEnvironment env)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _env= env;
        }
      
        public async Task<LoginResponseDTO> GetUserLoginDataAsync(LoginDTO loginDTO)
        {
            var users = await _userRepository.FindAsync(u =>
                (u.UserName == loginDTO.UsernameOrEmail || u.Email == loginDTO.UsernameOrEmail) &&
                u.Pass == loginDTO.Password);

            var user = users.FirstOrDefault();
            if (user == null)
                return null;

            var token = _jwtService.GenerateJwtToken(user.UserName,user.RoleName);

            return new LoginResponseDTO
            {
                Token = token,
                User = new UserDataDTO
                {
                    _id = user.UserId.ToString(),
                    Name = user.UserName,
                    Email = user.Email,
                    Role = user.RoleName
                }
            };
        }
        public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            var users = await _userRepository.FindAsync(u =>
                 (u.UserName == loginDTO.UsernameOrEmail || u.Email == loginDTO.UsernameOrEmail) &&
                 u.Pass == loginDTO.Password);

            var user = users.FirstOrDefault();
            if (user == null)
                return null;

            var token = _jwtService.GenerateJwtToken(user.UserName, user.RoleName);

            int? landlordStatus = null;
            if (user.RoleName == "landlord")
            {
                var landlord = await _userRepository.GetByIdAsync(user.UserId);
                landlordStatus = landlord?.FlagWaitingUser;
            }

            return new LoginResponseDTO
            {
                Token = token,
                User = new UserDataDTO
                {
                    _id = user.UserId.ToString(),
                    Name = user.UserName,
                    Email = user.Email,
                    Role = user.RoleName,
                    LandlordStatus = user.FlagWaitingUser
                }
            };
        }
        public async Task<RegisterResponseDTO> Register(RegisterDTO registerDto)
        {

            var usernameExists = (await _userRepository.FindAsync(u => u.UserName == registerDto.Username)).Any();
            if (usernameExists)
                throw new Exception("Username already exists");

            var emailExists = (await _userRepository.FindAsync(u => u.Email == registerDto.Email)).Any();
            if (emailExists)
                throw new Exception("Email already exists");

            string? filePath = null;

            if (registerDto.Role_name == "landlord" && registerDto.File != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uploadPath = Path.Combine(_env.ContentRootPath, "../Media");
                Directory.CreateDirectory(uploadPath);

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(registerDto.File.FileName);
                filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await registerDto.File.CopyToAsync(stream);
                }
            }
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                Pass = registerDto.Password,
                RoleName = registerDto.Role_name,
                FlagWaitingUser = registerDto.Role_name == "landlord" ? 1 : 0,
                FName = string.Empty, 
                LName = string.Empty,
                FilePath = filePath
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            //var kafkaMessage = new
            //{
            //    username = registerDto.Username,
            //    email = registerDto.Email,
            //    status = registerDto.Role_name == "landlord" ? "pending" : "succes",
            //    DateMessage = DateTime.Now,
               
            //};


            //string jsonMessage = JsonConvert.SerializeObject(kafkaMessage);

            //// 3. Send to Kafka topic "messages"
            //var config = new ProducerConfig
            //{
            //    BootstrapServers = "localhost:9092"
            //};

            //using var producer = new ProducerBuilder<Null, string>(config).Build();
            //await producer.ProduceAsync("WelcomeEmail", new Message<Null, string> { Value = jsonMessage }); // we send

            ////return Ok(new { status = "Message saved and sent to Kafka" });
             

            return new RegisterResponseDTO
            {
                UserId = user.UserId,
                Username = user.UserName,
                Email = user.Email,
                Role = user.RoleName,
                FlagWaitingUser = user.FlagWaitingUser,
            };
        }
    }
}