﻿using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Bus;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorkflowApp
{
    public class WorkflowInit
    {
        private static volatile WorkflowRuntime _runtime;
        private static readonly object _sync = new object();

        public static WorkflowRuntime Runtime
        {
            get
            {
                if (_runtime == null)
                {
                    lock (_sync)
                    {
                        if (_runtime == null)
                        {
                            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                            var builder = new WorkflowBuilder<XElement>(
                                new OptimaJet.Workflow.DbPersistence.DbXmlWorkflowGenerator(connectionString),
                                new OptimaJet.Workflow.Core.Parser.XmlWorkflowParser(),
                                new OptimaJet.Workflow.DbPersistence.DbSchemePersistenceProvider(connectionString)
                                ).WithDefaultCache();

                            _runtime = new WorkflowRuntime(new Guid("{8D38DB8F-F3D5-4F26-A989-4FDD40F32D9D}"))
                                .WithBuilder(builder)
                                .WithPersistenceProvider(new OptimaJet.Workflow.DbPersistence.DbPersistenceProvider(connectionString))
                                .WithTimerManager(new TimerManager())
                                .WithBus(new NullBus())
                                .SwitchAutoUpdateSchemeBeforeGetAvailableCommandsOn()
                                .Start();
                        }
                    }
                }

                return _runtime;
            }
        }
    }
}
