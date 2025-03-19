import 'package:dio/dio.dart';
import 'package:holefeeder/core/models/account.dart';
import 'package:holefeeder/core/models/category.dart';
import 'package:holefeeder/core/models/make_purchase.dart';
import 'package:retrofit/retrofit.dart';

part 'rest_client.g.dart';

@RestApi()
abstract class RestClient {
  factory RestClient(Dio dio, {String baseUrl}) = _RestClient;

  @GET('api/v2/accounts')
  Future<HttpResponse<List<Account>>> getAccounts(
    @Query('sort') List<String> sort,
  );

  @GET('api/v2/accounts/{id}')
  Future<HttpResponse<Account>> getAccount(@Path('id') String id);

  @GET('api/v2/categories')
  Future<HttpResponse<List<Category>>> getCategories();

  @GET('api/v2/categories/{id}')
  Future<HttpResponse<Category>> getCategory(@Path('id') String id);

  @POST('api/v2/transactions/make-purchase')
  Future<HttpResponse<String>> makePurchase(@Body() MakePurchase command);
}
