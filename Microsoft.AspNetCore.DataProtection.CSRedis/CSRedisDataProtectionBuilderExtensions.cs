using System;
using CSRedis;
using Microsoft.AspNetCore.DataProtection.CSRedis;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// Contains Redis-specific extension methods for modifying a <see cref="IDataProtectionBuilder"/>.
    /// </summary>
    public static class CsRedisDataProtectionBuilderExtensions
    {
        private const string DataProtectionKeysName = "DataProtection-Keys";

        /// <summary>
        /// Configures the data protection system to persist keys to specified key in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="redisFactory">The delegate used to create <see cref="CSRedisClient"/> instances.</param>
        /// <param name="key">The used to store key list.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToCsRedis(this IDataProtectionBuilder builder,
            Func<CSRedisClient> redisFactory, string key)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (redisFactory == null)
            {
                throw new ArgumentNullException(nameof(redisFactory));
            }

            return PersistKeysToCsRedisInternal(builder, redisFactory, key);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the default key ('DataProtection-Keys') in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="redisClient">The <see cref="CSRedisClient"/> for database access.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToCsRedis(this IDataProtectionBuilder builder,
            CSRedisClient redisClient)
        {
            return PersistKeysToCsRedis(builder, redisClient, DataProtectionKeysName);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified key in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="redisClient">The <see cref="CSRedisClient"/> for database access.</param>
        /// <param name="key">The used to store key list.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToCsRedis(this IDataProtectionBuilder builder,
            CSRedisClient redisClient, string key)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (redisClient == null)
            {
                throw new ArgumentNullException(nameof(redisClient));
            }

            RedisHelper.Initialization(redisClient);
            return PersistKeysToCsRedisInternal(builder, () => RedisHelper.Instance, key);
        }

        private static IDataProtectionBuilder PersistKeysToCsRedisInternal(IDataProtectionBuilder builder,
            Func<CSRedisClient> redisClient, string key)
        {
            builder.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new CsRedisXmlRepository(redisClient, key);
            });
            return builder;
        }
    }
}
