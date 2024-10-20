param applicationName string = 'bachelor13'
param keyVaultName string = '${applicationName}-kv'
param webAppName string = '${applicationName}-wa'
param frontWebAppName string = '${applicationName}-fwa'
param location string = 'West Europe'
param cosmosDbName string = '${applicationName}-db'
param appServicePlanName string = '${applicationName}-asp'



var skuDefault = {
  name: 'B1'
  capacity: 1
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
   location: location
   name: keyVaultName
   properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenant().tenantId
    publicNetworkAccess: 'Enabled'
    enableSoftDelete: true
    enablePurgeProtection: true
    softDeleteRetentionInDays: 90
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    accessPolicies: []
   }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
    name: appServicePlanName
    location: location
    properties: {
      reserved: false
    }
    sku: skuDefault
    kind: 'windows'
}

resource appService 'Microsoft.Web/sites@2020-06-01' = {
    location: location
    name: webAppName
    properties: {
      serverFarmId: appServicePlan.id
    }
}

resource appServiceFront 'Microsoft.Web/sites@2020-06-01' = {
    location: location
    name: frontWebAppName
    properties: {
      serverFarmId: appServicePlan.id
    }
}

// resource secretFaceServiceAPIKey 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
//     parent: keyVault
//     name: 'FaceServiceAPIKey'
//     properties: {
//       value: '6920e2f5fcfb45dd897f754d60fb7bc8'
//     }
// }
