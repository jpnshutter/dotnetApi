namespace TodoApi.Models
{
    public class User{
        public int Id{get;set;}
        public string Username{get;set;}
        public string Password{get;set;}
        public string Role{get;set;}

    }

    public class Signin{
        public string Username{get;set;}
        public string Password{get;set;}
    }

    public class Signup{
        public string Username{get;set;}
        public string Password{get;set;}

        public string Role{get;set;}

    }




}