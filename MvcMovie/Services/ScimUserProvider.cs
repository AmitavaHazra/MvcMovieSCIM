// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using MvcMovie.Data;

namespace Microsoft.SCIM.WebHostSample.Provider
{
    using Microsoft.SCIM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class ScimUserProvider : ProviderBase
    {
        private readonly UserDataContext _context;

        public ScimUserProvider(UserDataContext context)
        {
            _context = context;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser user = resource as Core2EnterpriseUser;
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            IEnumerable<Core2EnterpriseUser> exisitingUsers =
                _context.Users.Select(u => (Core2EnterpriseUser) u).ToList();
            if
            (
                exisitingUsers.Any(
                    (Core2EnterpriseUser exisitingUser) =>
                        string.Equals(exisitingUser.UserName, user.UserName, StringComparison.Ordinal))
            )
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var modelUser = (MvcMovie.Models.User) user;
            _context.Users.Add(modelUser);
            _context.SaveChanges();

            resource.Identifier = modelUser.Id.ToString();

            return Task.FromResult(resource);
        }

        public override async Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string identifier = resourceIdentifier.Identifier;

            if (int.TryParse(identifier, out int id) && _context.Users.Find(id) is MvcMovie.Models.User deleteUser)
            {
                _context.Users.Remove(deleteUser);
                await _context.SaveChangesAsync();
            }
        }

        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (null == parameters.AlternateFilters)
            {
                throw new ArgumentException("invalid request");
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier))
            {
                throw new ArgumentException("invalid request");
            }

            Resource[] results;
            IFilter queryFilter = parameters.AlternateFilters.SingleOrDefault();

            IEnumerable<Core2EnterpriseUser> allUsers = _context.Users
                .Select(u => (Core2EnterpriseUser)u);
            if (queryFilter == null)
            {
                results =
                    allUsers.ToList().Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return Task.FromResult(results);
            }

            if (string.IsNullOrWhiteSpace(queryFilter.AttributePath))
            {
                throw new ArgumentException("Invalid attribute query");
            }

            if (string.IsNullOrWhiteSpace(queryFilter.ComparisonValue))
            {
                throw new ArgumentException("Invalid comparison value");
            }

            if (queryFilter.FilterOperator != ComparisonOperator.Equals)
            {
                throw new NotSupportedException("Invalid comparison operator");
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.UserName))
            {
                results =
                    allUsers.ToList().Where(
                        (Core2EnterpriseUser item) =>
                           string.Equals(
                                item.UserName,
                               parameters.AlternateFilters.Single().ComparisonValue,
                               StringComparison.OrdinalIgnoreCase))
                               .Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return Task.FromResult(results);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.ExternalIdentifier))
            {
                results =
                    allUsers.ToList().Where(
                        (Core2EnterpriseUser item) =>
                           string.Equals(
                                item.ExternalIdentifier,
                               parameters.AlternateFilters.Single().ComparisonValue,
                               StringComparison.OrdinalIgnoreCase))
                               .Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return Task.FromResult(results);
            }

            throw new NotSupportedException("Unsupported filter");
        }

        public override async Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser user = resource as Core2EnterpriseUser;

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            IEnumerable<Core2EnterpriseUser> existingUsers = _context.Users
                .Select(u => (Core2EnterpriseUser)u).ToList();

            if
            (
                existingUsers.Any(
                    (Core2EnterpriseUser existingUser) =>
                        string.Equals(existingUser.UserName, user.UserName, StringComparison.Ordinal) &&
                        !string.Equals(existingUser.Identifier, user.Identifier, StringComparison.OrdinalIgnoreCase))
            )
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            if (int.TryParse(user.Identifier, out int id) && _context.Users.Find(id) is MvcMovie.Models.User existingModelUser)
            {
                existingModelUser.DisplayName = user.DisplayName;
                existingModelUser.Active = user.Active;
                existingModelUser.UserName = user.UserName;

                _context.Update(existingModelUser);
                await _context.SaveChangesAsync();
                return user;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters?.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            string identifier = parameters.ResourceIdentifier.Identifier;

            if (int.TryParse(identifier, out int id) && _context.Users.Find(id) is MvcMovie.Models.User modelUser)
            {
                return Task.FromResult<Resource>((Core2EnterpriseUser)modelUser);
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public override async Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (null == patch)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier)
            {
                throw new ArgumentException("Invalid Patch");
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException("Invalid Patch");
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException("Invalid Patch");
            }

            PatchRequest2 patchRequest =
                patch.PatchRequest as PatchRequest2;

            if (null == patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            if (int.TryParse(patch.ResourceIdentifier.Identifier, out int id) && _context.Users.Find(id) is MvcMovie.Models.User modelUser)
            {
                Core2EnterpriseUser scimUser = (Core2EnterpriseUser)modelUser;
                scimUser.Apply(patchRequest);
                await ReplaceAsync(scimUser, correlationIdentifier);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}
