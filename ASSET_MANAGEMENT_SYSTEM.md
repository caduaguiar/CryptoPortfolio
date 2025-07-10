# Comprehensive Asset Management System

This document describes the major transformation of the CryptoPortfolio application from a cryptocurrency-only system to a comprehensive asset management platform.

## Overview

The application has been completely redesigned to support multiple asset types including:
- **Cryptocurrencies** (Bitcoin, Ethereum, etc.)
- **Stocks** (Apple, Tesla, etc.)
- **Real Estate** (Houses, apartments, land)
- **Vehicles** (Cars, motorcycles, boats)
- **Commodities** (Gold, silver, oil)
- **Collectibles** (Art, antiques, trading cards)

## Key Features

### 🎯 Multi-Asset Portfolio Management
- Track different types of assets in organized portfolios
- Support for multiple portfolios per user
- Asset allocation analysis across different asset types
- Percentage breakdown of total portfolio value

### 📊 Advanced Transaction Tracking
- **Buy/Sell** transactions for tradeable assets
- **Deposit/Withdrawal** for account movements
- **Dividend** payments from stocks
- **Maintenance** costs for physical assets
- **Improvement** investments (renovations, upgrades)
- **Valuation** updates for current market values
- **Fee** tracking for transaction costs

### 📈 Analytics & Insights
- Average purchase price calculation
- Profit/loss tracking per asset
- Portfolio growth visualization
- Asset allocation by type
- Complete transaction history

### 🔄 Flexible Asset Management
- Add assets with detailed information (location, notes, descriptions)
- Update current values manually or automatically
- Track quantity changes over time
- Multi-currency support

## Database Schema

### New Tables

#### Users
```sql
- id (Primary Key)
- username (Unique)
- email (Unique)
- password_hash
- created_at
- last_updated
```

#### AssetTypes (Pre-seeded)
```sql
- id (Primary Key)
- name (Cryptocurrency, Stock, Real Estate, Vehicle, Commodity, Collectibles)
- description
```

#### Assets
```sql
- id (Primary Key)
- portfolio_id (Foreign Key)
- asset_type_id (Foreign Key)
- name
- symbol (optional)
- description
- quantity
- acquisition_date
- acquisition_cost
- current_value
- currency
- location (optional)
- notes
- created_at
- last_updated
- is_active
```

### Modified Tables

#### Portfolios
```sql
- id (Primary Key)
- user_id (Foreign Key) - NEW
- name - NEW
- description - NEW
- base_currency - NEW
- created_at - NEW
- last_updated
- is_active - NEW
```

#### Transactions
```sql
- id (Primary Key)
- asset_id (Foreign Key) - NEW
- portfolio_id (Foreign Key) - NEW
- transaction_type (Expanded enum)
- amount
- price_per_unit (optional)
- total_transaction_value - NEW
- transaction_currency - NEW
- transaction_date
- notes - NEW
- created_at - NEW
```

## API Endpoints

### Portfolio Management
- `GET /api/portfolio/dashboard` - Get comprehensive dashboard
- `GET /api/portfolio/transactions` - Get all transactions
- `POST /api/portfolio/transactions` - Add new transaction
- `DELETE /api/portfolio/transactions/{id}` - Delete transaction

### Asset Management
- `GET /api/assets/portfolio/{portfolioId}` - Get assets by portfolio
- `GET /api/assets/{id}` - Get specific asset
- `POST /api/assets` - Create new asset
- `PUT /api/assets/{id}` - Update asset
- `DELETE /api/assets/{id}` - Delete asset (soft delete)
- `GET /api/assets/type/{assetTypeId}` - Get assets by type
- `PUT /api/assets/{id}/current-value` - Update current value
- `GET /api/assets/{id}/average-price` - Get average purchase price
- `GET /api/assets/{id}/transactions` - Get asset transaction history

## Transaction Types

| Type | ID | Description | Use Case |
|------|----|-----------|---------| 
| Buy | 1 | Purchase of asset | Buying stocks, crypto |
| Sell | 2 | Sale of asset | Selling stocks, crypto |
| Deposit | 3 | Adding to position | Adding crypto to wallet |
| Withdrawal | 4 | Removing from position | Withdrawing crypto |
| Fee | 5 | Transaction fees | Trading fees, gas fees |
| Dividend | 6 | Dividend payments | Stock dividends |
| Maintenance | 7 | Maintenance costs | Property repairs, car service |
| Improvement | 8 | Asset improvements | Home renovations, car upgrades |
| Valuation | 9 | Value updates | Property appraisals |

## Usage Examples

### 1. Creating Assets

#### Cryptocurrency Asset
```json
{
  "portfolioId": 1,
  "assetTypeId": 1,
  "name": "Bitcoin",
  "symbol": "BTC",
  "quantity": 0.5,
  "acquisitionDate": "2024-01-15T00:00:00Z",
  "acquisitionCost": 25000.00,
  "currentValue": 30000.00,
  "currency": "USD"
}
```

#### Real Estate Asset
```json
{
  "portfolioId": 1,
  "assetTypeId": 3,
  "name": "Downtown Apartment",
  "description": "2-bedroom apartment",
  "quantity": 1,
  "acquisitionDate": "2023-06-15T00:00:00Z",
  "acquisitionCost": 250000.00,
  "currentValue": 275000.00,
  "currency": "USD",
  "location": "123 Main St, Downtown"
}
```

### 2. Adding Transactions

#### Buy Transaction
```json
{
  "assetId": 1,
  "portfolioId": 1,
  "transactionType": 1,
  "amount": 0.1,
  "pricePerUnit": 60000.00,
  "totalTransactionValue": 6000.00,
  "transactionCurrency": "USD",
  "transactionDate": "2024-03-01T00:00:00Z"
}
```

#### Maintenance Transaction
```json
{
  "assetId": 3,
  "portfolioId": 1,
  "transactionType": 7,
  "amount": 0,
  "totalTransactionValue": 1500.00,
  "transactionCurrency": "USD",
  "transactionDate": "2024-02-10T00:00:00Z",
  "notes": "HVAC system repair"
}
```

## Migration Guide

### From Old System
The migration automatically:
1. Creates new tables (Users, AssetTypes, Assets)
2. Modifies existing tables (Portfolios, Transactions)
3. Seeds asset types
4. Preserves existing cryptocurrency data

### Database Migration
```bash
cd CryptoPortfolio.API
dotnet ef migrations add ComprehensiveAssetManagementSystem
dotnet ef database update
```

## Key Benefits

### For Users
- **Unified View**: See all assets in one place
- **Better Analytics**: Understand asset allocation and performance
- **Flexible Tracking**: Support for any type of asset
- **Detailed History**: Complete transaction history with context

### For Developers
- **Extensible Design**: Easy to add new asset types
- **Clean Architecture**: Separated concerns with proper DTOs
- **Comprehensive API**: Full CRUD operations for all entities
- **Type Safety**: Strong typing with enums and validation

## Future Enhancements

### Planned Features
- **Automated Price Updates**: Integration with multiple price APIs
- **Portfolio Rebalancing**: Suggestions for optimal allocation
- **Tax Reporting**: Generate tax reports for different jurisdictions
- **Performance Analytics**: Advanced charts and metrics
- **Mobile App**: React Native mobile application
- **Import/Export**: CSV import/export functionality

### Technical Improvements
- **Authentication**: JWT-based user authentication
- **Caching**: Redis caching for better performance
- **Background Jobs**: Automated price updates and calculations
- **API Versioning**: Support for multiple API versions
- **Rate Limiting**: API rate limiting and throttling

## Testing

Use the provided `AssetManagement.http` file to test all endpoints. The file includes examples for:
- Creating different types of assets
- Adding various transaction types
- Retrieving portfolio data and analytics
- Updating asset values
- Managing the complete asset lifecycle

## Support

For questions or issues:
1. Check the API documentation in Swagger UI
2. Review the HTTP examples file
3. Examine the migration files for database changes
4. Test with the provided sample data

---

**Note**: This system maintains backward compatibility with existing cryptocurrency data while providing a foundation for comprehensive asset management across all asset classes.
