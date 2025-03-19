import 'dart:convert';

import 'package:flutter_secure_storage/flutter_secure_storage.dart'; // For secure token storage
import 'package:http/http.dart' as http;

class ApiClient<T> {
  final String baseUrl;
  final http.Client client;
  final FlutterSecureStorage storage =
      const FlutterSecureStorage(); // Secure storage

  ApiClient(this.baseUrl, {http.Client? client})
    : client = client ?? http.Client();

  Future<String?> _getAccessToken() async {
    // Auth0 auth0 = Auth0(auth0Domain, auth0ClientId);

    // final credentials = await auth0.credentialsManager.credentials(
    //   parameters: {'audience': auth0Audience},
    // );
    // final String accessToken = credentials.accessToken;
    return await storage.read(key: 'access_token');
  }

  Future<Map<String, String>> _getAuthHeaders() async {
    final accessToken = await _getAccessToken();
    if (accessToken == null) {
      return {}; // Return empty headers if no token
    }
    return {
      'Authorization': 'Bearer $accessToken',
      'Content-Type': 'application/json', // Or whatever your API expects
    };
  }

  Future<T> get(String endpoint, {Map<String, String>? customHeaders}) async {
    final url = Uri.parse('$baseUrl/$endpoint');
    final authHeaders = await _getAuthHeaders();
    final allHeaders = {...authHeaders, ...?customHeaders}; // Combine headers
    try {
      final response = await client.get(url, headers: allHeaders);
      return _handleResponse(response);
    } catch (e) {
      throw Exception('Network error: $e');
    }
  }

  Future<T> post(
    String endpoint,
    dynamic body, {
    Map<String, String>? customHeaders,
  }) async {
    final url = Uri.parse('$baseUrl/$endpoint');
    final authHeaders = await _getAuthHeaders();
    final allHeaders = {...authHeaders, ...?customHeaders}; // Combine headers

    try {
      final response = await client.post(
        url,
        headers: allHeaders,
        body: json.encode(body),
      );
      return _handleResponse(response);
    } catch (e) {
      throw Exception('Network error: $e');
    }
  }

  // Add other HTTP methods (put, delete, etc.) as needed

  T _handleResponse(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return _decodeResponse(response.body);
    } else {
      throw Exception(
        'Failed to load data: ${response.statusCode} ${response.body}',
      );
    }
  }

  T _decodeResponse(String body) {
    // Implement your decoding logic here.
    return json.decode(body) as T;
  }
}
