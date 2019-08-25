﻿using HotChocholateExpandable.Database;
using HotChocholateExpandable.Models;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HotChocholateExpandable
{
    public class Startup
    {
        readonly IConfiguration Config;

        public Startup(IConfiguration config)
        {
            Config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<DatabaseContext>(opts => opts.UseSqlite(Config["Db"]).EnableSensitiveDataLogging(true));
            services.AddGraphQL(sp => Schema.Create(opts =>
            {
                opts.RegisterServiceProvider(sp);

                //opts.RegisterQueryType<RootQuery>();
                //opts.RegisterType<RootQueryObjectType>();
                //opts.RegisterType<UserObjectType>();
                //opts.RegisterType<UserRolesObjectType>();
                //opts.RegisterType<RoleObjectType>();
            }));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DatabaseContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var initialData = new Blog
            {
                Id = Guid.NewGuid(),
                Name = "Initial Blog",
                Owner = new User
                {
                    Email = "sample@user.com",
                    Id = 1,
                    Name = "Sample",
                    Surname = "User",
                },
                BlogPosts = new List<BlogPost> {
                    new BlogPost
                    {
                        Id = 1,
                        Title = "First Post",
                        Description = "First Description",
                        Content = "Some blog post content",
                        Comments = new List<Comment>
                        {
                            new Comment
                            {
                                Id = Guid.NewGuid(),
                                Text = "This is awful",
                                OwnerId = 1
                            }
                        },
                        BlogPostTags = new List<BlogPostTag>
                        {
                            new BlogPostTag
                            {
                                Tag = new Tag
                                {
                                    Id = 1,
                                    TagName = "Code"
                                }
                            },
                            new BlogPostTag
                            {
                                Tag = new Tag
                                {
                                    Id = 2,
                                    TagName = "C#"
                                }
                            }
                        }
                    }
                }
            };

            app.UseMvc();
            app.UseGraphQL("/graphql");
            app.UsePlayground("/graphql", "/playground");
            app.UseVoyager("/graphql", "/voyager");
        }
    }
}