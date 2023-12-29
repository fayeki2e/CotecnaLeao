using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Techparva.GenericRepository;
using TechParvaLEAO.Models;
using Newtonsoft.Json;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace TechParvaLEAO.Areas.Organization.Models
{

    public partial class Team : Entity<int>, IAggregateRoot
    {


        public Team()
        {



        }
        public int Id { get; set; }
     
        public string TeamName { get; set; }
        public bool Deactivated { get; set; } = false;

    }
}
