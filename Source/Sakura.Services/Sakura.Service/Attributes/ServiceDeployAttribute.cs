﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakura.Service
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceDeployAttribute : Attribute
    {
        public ServiceDeployAttribute(DeployEnvironment Deployment)
        {
            this.Deployment = Deployment;
        }
        public DeployEnvironment Deployment { get; }
    }
}
