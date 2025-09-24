# Search Foods and Combos API

## Mô tả

API này cho phép tìm kiếm Food và Combo theo tên kèm theo đầy đủ thông tin Images và Promotion. API hỗ trợ tìm kiếm không phân biệt hoa thường và tìm kiếm từ khóa một phần.

## Endpoints

### 1. Get All Foods and Combos (No Search)

**GET** `/api/food/all-foods-and-combos`

### 2. Search Foods and Combos by Name

**GET** `/api/food/search-foods-and-combos`

## Authorization

- **Không yêu cầu authentication** - API public cho hiển thị sản phẩm

## Request Parameters

### Search API Parameters

| Parameter | Type   | Required | Description                                          |
| --------- | ------ | -------- | ---------------------------------------------------- |
| `name`    | string | No       | Tên hoặc từ khóa để tìm kiếm trong tên Food và Combo |

### Examples:

- `/api/food/search-foods-and-combos?name=bánh mì` - Tìm tất cả có chứa "bánh mì"
- `/api/food/search-foods-and-combos?name=combo` - Tìm tất cả có chứa "combo"
- `/api/food/search-foods-and-combos` - Lấy tất cả (không tìm kiếm)

## Response

### Success Response (200 OK)

```json
{
  "success": true,
  "message": "Tìm kiếm 'bánh mì' thành công",
  "searchTerm": "bánh mì",
  "data": {
    "foods": [
      {
        "foodId": 1,
        "foodCategoryId": 1,
        "foodName": "Bánh mì thịt nướng",
        "slug": "banh-mi-thit-nuong",
        "description": "Bánh mì với thịt nướng thơm ngon",
        "price": 25000,
        "status": true,
        "quantity": 100,
        "sold": 50,
        "createdAt": "2024-01-01T10:00:00Z",
        "images": {
          "imageId": 1,
          "imageName": "banh-mi-thit-nuong.jpg",
          "imageUrl": "https://example.com/images/banh-mi-thit-nuong.jpg",
          "alt": "Bánh mì thịt nướng"
        },
        "foodCategory": {
          "categoryId": 1,
          "categoryName": "Bánh mì",
          "description": "Các loại bánh mì"
        },
        "promotion": {
          "promotionId": 1,
          "promotionName": "Giảm giá 10%",
          "discountPercent": 10,
          "startDate": "2024-01-01T00:00:00Z",
          "endDate": "2024-12-31T23:59:59Z",
          "status": true
        }
      }
    ],
    "combos": [
      {
        "comboId": 1,
        "comboName": "Combo bánh mì + nước",
        "slug": "combo-banh-mi-nuoc",
        "foodCategoryId": 2,
        "price": 35000,
        "description": "Combo tiết kiệm bánh mì kèm nước uống",
        "status": true,
        "quantity": 50,
        "sold": 25,
        "createdAt": "2024-01-01T10:00:00Z",
        "images": {
          "imageId": 2,
          "imageName": "combo-banh-mi-nuoc.jpg",
          "imageUrl": "https://example.com/images/combo-banh-mi-nuoc.jpg",
          "alt": "Combo bánh mì nước"
        },
        "foodCategorys": {
          "categoryId": 2,
          "categoryName": "Combo",
          "description": "Các combo tiết kiệm"
        },
        "promotion": {
          "promotionId": 2,
          "promotionName": "Giảm giá 15%",
          "discountPercent": 15,
          "startDate": "2024-01-01T00:00:00Z",
          "endDate": "2024-12-31T23:59:59Z",
          "status": true
        }
      }
    ],
    "totalFoods": 1,
    "totalCombos": 1,
    "totalItems": 2
  }
}
```

### No Results Response (200 OK)

```json
{
  "success": true,
  "message": "Tìm kiếm 'xyz123' thành công",
  "searchTerm": "xyz123",
  "data": {
    "foods": [],
    "combos": [],
    "totalFoods": 0,
    "totalCombos": 0,
    "totalItems": 0
  }
}
```

## Error Responses

### 500 Internal Server Error

```json
{
  "success": false,
  "message": "Có lỗi xảy ra khi tìm kiếm food và combo",
  "detail": "Chi tiết lỗi..."
}
```

## Ví dụ sử dụng

### 1. Tìm kiếm theo từ khóa "bánh mì"

```http
GET /api/food/search-foods-and-combos?name=bánh mì
```

**Response:**

```json
{
  "success": true,
  "message": "Tìm kiếm 'bánh mì' thành công",
  "searchTerm": "bánh mì",
  "data": {
    "foods": [...], // Foods có tên chứa "bánh mì"
    "combos": [...], // Combos có tên chứa "bánh mì"
    "totalFoods": 3,
    "totalCombos": 2,
    "totalItems": 5
  }
}
```

### 2. Tìm kiếm không phân biệt hoa thường

```http
GET /api/food/search-foods-and-combos?name=COMBO
```

**Response:**

```json
{
  "success": true,
  "message": "Tìm kiếm 'COMBO' thành công",
  "searchTerm": "COMBO",
  "data": {
    "foods": [],
    "combos": [...], // Tất cả combos có tên chứa "combo" (case-insensitive)
    "totalFoods": 0,
    "totalCombos": 5,
    "totalItems": 5
  }
}
```

### 3. Tìm kiếm không có tham số (lấy tất cả)

```http
GET /api/food/search-foods-and-combos
```

**Response:**

```json
{
  "success": true,
  "message": "Lấy danh sách food và combo thành công",
  "searchTerm": null,
  "data": {
    "foods": [...], // Tất cả foods
    "combos": [...], // Tất cả combos
    "totalFoods": 20,
    "totalCombos": 10,
    "totalItems": 30
  }
}
```

## Search Logic

### Tìm kiếm Features:

1. **Case Insensitive**: Không phân biệt hoa thường (`ToLower()`)
2. **Partial Match**: Tìm kiếm từ khóa một phần (`Contains()`)
3. **Both Foods & Combos**: Tìm trong cả hai loại sản phẩm
4. **Active Items Only**: Chỉ tìm trong sản phẩm đang hoạt động

### Search Fields:

- **Foods**: Tìm trong `FoodName`
- **Combos**: Tìm trong `ComboName`

### Edge Cases:

- `name = null`: Trả về tất cả sản phẩm
- `name = ""`: Trả về tất cả sản phẩm
- `name = "   "` (whitespace): Trả về tất cả sản phẩm
- No matches: Trả về mảng rỗng với `totalItems = 0`

## Performance Considerations

### Database Query Optimization:

- **Foods**: Database-level filtering với LINQ `Where`
- **Combos**: Memory-level filtering sau khi lấy từ service
- **Eager Loading**: Include Images và Promotion trong single query

### Search Performance:

- **Database Level**: Foods search sử dụng SQL LIKE operator
- **Memory Level**: Combos search sử dụng C# string operations
- **Index Recommendation**: Tạo index trên `FoodName` và `ComboName` columns

## Use Cases

### Frontend Search:

- **Real-time Search**: Tìm kiếm khi user gõ
- **Search Suggestions**: Gợi ý sản phẩm
- **Category Filter + Search**: Kết hợp với filter khác
- **Mobile Search**: Tối ưu cho mobile interface

### Business Applications:

- **Inventory Management**: Tìm sản phẩm theo tên
- **Order Management**: Hỗ trợ staff tìm sản phẩm
- **Analytics**: Theo dõi từ khóa search phổ biến
- **SEO**: Tối ưu cho search engine

## API Comparison

| API             | Endpoint                            | Purpose                  | Search Capability |
| --------------- | ----------------------------------- | ------------------------ | ----------------- |
| **Get All**     | `/api/food/all-foods-and-combos`    | Lấy tất cả sản phẩm      | ❌ No search      |
| **Search**      | `/api/food/search-foods-and-combos` | Tìm kiếm theo tên        | ✅ Name search    |
| **Foods Only**  | `/api/food`                         | Lấy foods với pagination | ❌ Limited search |
| **Combos Only** | `/api/combo/combos`                 | Lấy combos               | ❌ No search      |

### Lợi ích của Search API:

- **Unified Search**: Tìm trong cả foods và combos
- **Complete Data**: Bao gồm images và promotions
- **Flexible**: Hỗ trợ cả search và get all
- **User Friendly**: Case-insensitive và partial matching

## Status Codes

- `200 OK`: Tìm kiếm thành công (có hoặc không có kết quả)
- `500 Internal Server Error`: Lỗi server

## Dependencies

- **FoodServices**: Service xử lý business logic và search
- **ComboServices**: Service lấy dữ liệu combo
- **AutoMapper**: Mapping entities sang DTOs
- **Entity Framework**: Database access với LINQ filtering
