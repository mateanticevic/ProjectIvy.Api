﻿using System;
using ProjectIvy.Common.Interfaces;

namespace ProjectIvy.Data.Services.AzureStorage;

public class AzureStorageFactory : IServiceFactory<IAzureStorageHelper>
{

    public IAzureStorageHelper Build() => new AzureStorageHelper(Environment.GetEnvironmentVariable("CONNECTION_STRING_AZURE_STORAGE"));
}
