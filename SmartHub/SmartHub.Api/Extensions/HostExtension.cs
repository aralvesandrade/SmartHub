﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmartHub.Infrastructure.Database;
using System;
using System.Threading.Tasks;

namespace SmartHub.Api.Extensions
{
	public static class HostExtension
	{
		public static IHost AsciiLogo(this IHost host)
		{
			Console.WriteLine($"" + "\r\n" +
				$@"  _   _               _   _                       " + "\r\n" +
				$@" | \ | | _____      _| | | | __ ___   _____ _ __  " + "\r\n" +
				$@" |  \| |/ _ \ \ /\ / / |_| |/ _` \ \ / / _ \ '_ \ " + "\r\n" +
				$@" | |\  |  __/\ V  V /|  _  | (_| |\ V /  __/ | | |" + "\r\n" +
				$@" |_| \_|\___| \_/\_/ |_| |_|\__,_| \_/ \___|_| |_|" + "\r\n" +
				$@"                                                  " + "\r\n" +
				$@"--------------------------------------------------" + "\r\n" +
				$"" + "\r\n");
			return host;
		}

		public static IHost WelcomeText(this IHost host)
		{
			Console.WriteLine($"Welcome to Newhaven, this is a smarthome written in c# and asp.net core." + "\n\r" +
				"This is a private project of mine and I use this to learn new things and create my own smarthome that " + "\n\r" +
				"I am going to use at home." + "\n\r" +
				"For more info and if you encounter any issues or have any feedback please visit: https://github.com/lTimeless/NewHaven_Server :)" + "\n\r" +
				"--------------------------------------------------");
			return host;
		}

		public static async Task<IHost> MigrateDatabase(this IHost host, bool deleteMode)
		{
			using var scope = host.Services.CreateScope();
			await using var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			var env = host.Services.GetRequiredService<IWebHostEnvironment>();
			try
			{
				// dev Mode
				if (env.IsDevelopment())
				{
					if (deleteMode)
					{
						Log.Information($"[MigrationManager] Deletedatabase");
						appContext.Database.EnsureDeleted();
					}
					Log.Information($"[MigrationManager] Update or Create database if needed");

					// adds the current Entity structure
					appContext.Database.EnsureCreated(); //creates or updates the db if neccassary
				}
				// prod mode
				else
				{
					Log.Information($"[MigrationManager] Update or Create database if needed");
					// adds the latest Migration from the Migrationsfolder
					await appContext.Database.MigrateAsync(); //creates or updates the db if neccassary
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"[MigrationManager] Error while migrating the DB on startup -- {ex.Message} \n {ex.Source}");
			}

			return host;
		}
	}
}