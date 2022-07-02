# CloudFabric.Auth Changelog

## 1.0.3

- Upgrade to .net6

## 1.0.2

- (bugfix): Validate all tenant access control groups instead of first one
- (bugfix): Instead of returning first Deny or Allow statements - gather all of them in a list and then check if there is any Deny statement. This improves security and makes things more logical - if something is denyed - that should be priority rule.

## 1.0.0

- Initial commit
