﻿global using System;
global using Dtat.Results.Server;
global using Infrastructure.Enums;
global using Infrastructure;
global using Infrastructure.Settings;
global using Microsoft.AspNetCore.Mvc;
global using Infrastructure.Attributes;
global using Infrastructure.Middlewares;
global using Microsoft.Extensions.Options;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Shared.Enums;
global using ViewModels.General;
global using System.Security.Claims;
global using Services;
global using Domain.UserManagment;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Hosting;
global using System.IO;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Builder;
global using System.Collections.Generic;
global using Dtat.Logging;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.OpenApi.Models;
global using Swashbuckle.AspNetCore.SchemaBuilder;
global using Swashbuckle.AspNetCore.SwaggerUI;
global using System.Reflection;
global using Infrastructure.Swagger;
global using Microsoft.Extensions.Configuration;
global using Persistence;
global using Microsoft.AspNetCore.Mvc.Controllers;
global using Pluralize.NET;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Microsoft.OpenApi.Any;
global using Microsoft.AspNetCore.Mvc.Authorization;
global using EasyCaching.Core;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;
global using Domain.SeedWork;
global using AutoMapper;
global using Dtat.Utilities;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;