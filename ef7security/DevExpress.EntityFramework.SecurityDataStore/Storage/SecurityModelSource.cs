using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace DevExpress.EntityFramework.SecurityDataStore.Storage {
    public class SecurityModelSource : ModelSource {
        public SecurityModelSource([NotNull] IDbSetFinder setFinder, 
                                   [NotNull] ICoreConventionSetBuilder coreConventionSetBuilder, 
                                   [NotNull] IModelCustomizer modelCustomizer, 
                                   [NotNull] IModelCacheKeyFactory modelCacheKeyFactory) : base(setFinder, coreConventionSetBuilder, modelCustomizer, modelCacheKeyFactory) {
        }
    }
}
