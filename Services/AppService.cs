using BSBoilerPlate.Authorization;
using BSBoilerPlate.Data;
using BSBoilerPlate.Models;
using BSBoilerPlate.Models.DTOs;
using BSBoilerPlate.Pages;
using BSBoilerPlate.Pages.Administration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static MudBlazor.CategoryTypes;

namespace BSBoilerPlate.Services
{
    public class AppService
    {
        public bool MenuOpened;

        public BSBoilerPlateContext _BSBoilerPlateContext;
        private CustomAuthenticationStateProvider _customAuthenticationStateProvider;

        public AppService(BSBoilerPlateContext BSBoilerPlateContext, AuthenticationStateProvider authenticationStateProvider)
        {
            _BSBoilerPlateContext = BSBoilerPlateContext;
            _customAuthenticationStateProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
        }

        public int SessionMaxIdleMinutes()
        {
            return 10;
        }

        public Guid SessionGUID { get; set; } = Guid.NewGuid();

        public List<ChatMessage> ChatMessageGet(int User1ID, int User2ID)
        {
            List<ChatMessage> ret = _BSBoilerPlateContext.ChatMessages
                               .Include(p => p.UserFrom)
                               .Include(p => p.UserTo)
                               .Where(p => (p.UserFromID == User1ID && p.UserToID == User2ID) || (p.UserFromID == User2ID && p.UserToID == User1ID))
                               .OrderByDescending(p => p.DateTime)
                               .Take(100)
                               .OrderBy(p => p.DateTime)
                               .ToList();

            return ret;
        }

        public bool ChatMessagesMarkRead(int UserToID, int UserFromID)
        {
            var chatMessages = _BSBoilerPlateContext.ChatMessages
                               .Where(p => (p.UserFromID == UserFromID && p.UserToID == UserToID && p.DateTimeRead == null))
                               .ToList();

            chatMessages.ForEach(a => a.DateTimeRead = DateTime.Now);

            _BSBoilerPlateContext.SaveChanges();

            return chatMessages.Count > 0 ? true : false;
        }

        public int ChatMessageGetUnreads(int UserToID)
        {
            int ret = _BSBoilerPlateContext.ChatMessages
                                .Where((p => p.UserToID == UserToID && p.DateTimeRead == null))
                                .Count();

            return ret;
        }

        public void ChatMessageSend(string message, int UserFromID, int UserToID)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.Message = message;
                chatMessage.UserFromID = UserFromID;
                chatMessage.UserToID = UserToID;
                chatMessage.DateTime = DateTime.Now;

                _BSBoilerPlateContext.ChatMessages.Add(chatMessage);
                _BSBoilerPlateContext.SaveChanges();

                Log(LogAction.Names.ChatMessageSend.ToString(), chatMessage);
            }
        }

        public Models.Application ApplicationGet()
        {
            return _BSBoilerPlateContext.Applications.First();
        }

        public List<Log> LastestsLogins(int UserID)
        {
            var logActionIDLogin = _BSBoilerPlateContext.LogActions.Where(p => p.Name == LogAction.Names.Login.ToString()).Select(p => p.ID).First();

            return _BSBoilerPlateContext.Logs
                               .Include(p => p.LogItems)
                               .Where(p => p.UserID == UserID && p.LogActionID == logActionIDLogin)
                               .OrderByDescending(p => p.DateTime)
                               .Take(10)
                               .ToList();
        }

        public async Task ApplicationUpdate(Models.Application application)
        {
            _BSBoilerPlateContext.Applications.Update(application);
            await _BSBoilerPlateContext.SaveChangesAsync();

            Log(LogAction.Names.ApplicationUpdate.ToString(), application);
        }

        public User UserGet(int ID)
        {
            return _BSBoilerPlateContext.Users.Find(ID);
        }

        public List<User> UsersGet()
        {
            var query = _BSBoilerPlateContext.Users
                            .Include(u => u.Role);

            return query.ToList();
        }

        public void UserUpdate(User user)
        {
            _BSBoilerPlateContext.Users.Update(user);
            _BSBoilerPlateContext.SaveChanges();

            Log(LogAction.Names.UserUpdate.ToString(), user);
        }

        public void UserInsert(ref User user)
        {
            _BSBoilerPlateContext.Users.Add(user);
            _BSBoilerPlateContext.SaveChanges();

            Log(LogAction.Names.UserInsert.ToString(), user);
        }

        public LogListDto LogsGet(LogDataGridRequestDto request)
        {
            LogListDto logList = new LogListDto();

            var query = _BSBoilerPlateContext.Logs
                            .Include(p => p.LogAction)
                            .Include(p => p.User)
                            .Include(p => p.LogItems)
                            .Select(p => new LogListItemDto
                            {
                                ID = p.ID,
                                DateTime = p.DateTime,
                                LogAction = p.LogAction.Name,
                                UserName = p.User.UserName,
                                FullName = $"{p.User.GivenName} {p.User.FirstSurname} {p.User.SecondSurname}".Trim(),
                                LogItems = p.LogItems
                            });

            if (request.FilterDefinitions != null && request.FilterDefinitions.Count() > 0)
            {
                foreach (var filter in request.FilterDefinitions)
                {
                    query = query.Where(filter.GenerateFilterFunction()).AsQueryable();
                }
            }

            if (request.SortDefinitions != null && request.SortDefinitions.Count() > 0)
            {
                foreach (var sort in request.SortDefinitions)
                {
                    query = query.OrderByS(sort.SortBy, sort.Descending);
                }
            }
            else
            {
                query = query.OrderBy(w => w.DateTime);
            }

            query = query.Skip(request.PageSize * request.Page)
                 .Take(request.PageSize);

            try
            {
                logList.Items = query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return logList;
        }

        public void Log(string logActionName)
        {
            Log(logActionName, null);
        }

        public void Log(string logActionName, ICollection<LogItem>? logItems)
        {
            LogAction logAction = (from la in _BSBoilerPlateContext.LogActions where la.Name == logActionName select la).First();
            User? user = UserLogged(_BSBoilerPlateContext);

            if (user != null && logAction != null)
            {
                Log log = new Log();
                log.LogActionID = logAction.ID;
                log.DateTime = DateTime.Now;
                log.UserID = user.ID;
                log.LogItems = logItems;

                _BSBoilerPlateContext.Add(log);
                _BSBoilerPlateContext.SaveChanges();
            }
        }

        public void Log(string logActionName, object affectedObject)
        {
            Type myType = affectedObject.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            HashSet<LogItem> logItems = new HashSet<LogItem>();

            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(affectedObject, null);

                if (propValue != null)
                {
                    LogItem logItem = new LogItem();
                    logItem.Name = prop.Name;
                    logItem.Value = propValue.ToString();
                    logItems.Add(logItem);
                }
            }

            Log(logActionName, logItems);
        }

        public Task<User?> UserLogin(string username, string passwordMD5) {

            var user = (from u in _BSBoilerPlateContext.Users where u.UserName == username && u.Password == passwordMD5 && u.Inactive == null select u).FirstOrDefaultAsync();

            return user;    
        }

        public User? UserLogged()
        {
            return UserLogged(_BSBoilerPlateContext);
        }

        public User? UserLogged(BSBoilerPlateContext ctx)
        {
            var userState = (_customAuthenticationStateProvider.GetAuthenticationStateAsync()).Result.User;

            User? user = null;

            if (userState != null && userState.Identity != null)
            {
                user = (from u in ctx.Users where u.UserName == userState.Identity.Name orderby u.ID select u).FirstOrDefault();
            }

            return user;
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name.Replace("_", " "));
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}