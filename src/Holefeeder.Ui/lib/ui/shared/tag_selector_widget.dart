import 'package:flutter/cupertino.dart';

// Import Tag model

class CupertinoTagSelector extends StatefulWidget {
  final Future<List<String>> futureTags;
  final List<String> initialTags;
  final ValueChanged<List<String>> onTagsChanged;

  const CupertinoTagSelector({
    required this.futureTags,
    required this.onTagsChanged,
    this.initialTags = const [],
    super.key,
  });

  @override
  State<CupertinoTagSelector> createState() => _CupertinoTagSelectorState();
}

class _CupertinoTagSelectorState extends State<CupertinoTagSelector> {
  late Future<List<String>> _futureTags;
  late List<String> _tags;
  late List<String> _filteredTags; // List to hold filtered tags
  late List<String> _allTags; // Store all tags for filtering
  final TextEditingController _textEditingController = TextEditingController();
  final FocusNode _textFieldFocusNode =
      FocusNode(); // FocusNode for the text field
  final FocusNode _listViewFocusNode =
      FocusNode(); // FocusNode for the ListView

  @override
  void initState() {
    super.initState();
    _textFieldFocusNode.addListener(_handleFocusChange);
    _listViewFocusNode.addListener(_handleFocusChange);
    _futureTags = widget.futureTags;
    _tags = List.from(widget.initialTags); // Initialize with initialTags
    _filteredTags = []; // Initialize filtered tags
    _loadTags();
  }

  void _handleFocusChange() {
    if (_textFieldFocusNode.hasFocus || _listViewFocusNode.hasFocus) {
      setState(() {
        _filteredTags =
            _allTags; // Show all tags when either component has focus
      });
    } else {
      setState(() {
        _filteredTags = []; // Clear filtered tags when neither has focus
      });
    }
  }

  @override
  void dispose() {
    _textFieldFocusNode.dispose(); // Dispose the text field FocusNode
    _listViewFocusNode.dispose(); // Dispose the ListView FocusNode
    _textEditingController.dispose();
    super.dispose();
  }

  Future<void> _loadTags() async {
    final tags = await _futureTags;
    setState(() {
      _allTags = tags; // Store all tags
      _tags =
          tags.where((tag) => widget.initialTags.any((t) => t == tag)).toList();
    });
  }

  void _filterTags(String query) {
    setState(() {
      if (query.isEmpty) {
        _filteredTags = [];
      } else {
        _filteredTags =
            _allTags // Filter from all tags
                .where((tag) => tag.toLowerCase().contains(query.toLowerCase()))
                .toList();
      }
    });
  }

  void _addTag(String tagName) {
    if (tagName.isNotEmpty && !_tags.any((tag) => tag == tagName)) {
      setState(() {
        _tags.add(tagName);
        _filteredTags = []; // Clear filtered tags
        _textEditingController.clear();
        widget.onTagsChanged(_tags);
      });
    }
  }

  void _selectTag(String tag) {
    if (!_tags.contains(tag)) {
      setState(() {
        _tags.add(tag);
        _filteredTags = []; // Clear filtered tags
        _textEditingController.clear();
        widget.onTagsChanged(_tags);
      });
    }
  }

  void _removeTag(String tag) {
    setState(() {
      _tags.remove(tag);
      widget.onTagsChanged(_tags);
    });
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<String>>(
      future: _futureTags,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return Center(child: CupertinoActivityIndicator());
        } else if (snapshot.hasError) {
          return Center(child: Text('Error loading tags'));
        } else {
          return FocusScope(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Wrap(
                  spacing: 8.0,
                  runSpacing: 4.0,
                  children:
                      _tags.map((tag) {
                        return CupertinoButton(
                          padding: EdgeInsets.zero,
                          onPressed: () => _removeTag(tag),
                          child: Container(
                            decoration: BoxDecoration(
                              color: CupertinoColors.systemGrey5,
                              borderRadius: BorderRadius.circular(8.0),
                            ),
                            padding: EdgeInsets.symmetric(
                              horizontal: 12.0,
                              vertical: 6.0,
                            ),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Text(
                                  tag,
                                  style: TextStyle(
                                    color: CupertinoColors.black,
                                  ),
                                ),
                                SizedBox(width: 4),
                                Icon(
                                  CupertinoIcons.clear_circled_solid,
                                  size: 16,
                                  color: CupertinoColors.systemGrey,
                                ),
                              ],
                            ),
                          ),
                        );
                      }).toList(),
                ),
                CupertinoTextField(
                  controller: _textEditingController,
                  focusNode:
                      _textFieldFocusNode, // Attach the text field FocusNode
                  placeholder: 'Add Tag',
                  suffix: CupertinoButton(
                    padding: EdgeInsets.zero,
                    child: Icon(CupertinoIcons.add_circled_solid),
                    onPressed: () {
                      _addTag(_textEditingController.text);
                    },
                  ),
                  onChanged: (value) {
                    _filterTags(value);
                  },
                  onSubmitted: (value) {
                    _addTag(value);
                  },
                ),
                if (_filteredTags.isNotEmpty)
                  Focus(
                    focusNode:
                        _listViewFocusNode, // Attach the ListView FocusNode
                    child: SizedBox(
                      height: 150.0,
                      child: ListView.builder(
                        itemCount: _filteredTags.length,
                        itemBuilder: (context, index) {
                          final tag = _filteredTags[index];
                          return CupertinoButton(
                            padding: EdgeInsets.zero,
                            onPressed: () {
                              _listViewFocusNode
                                  .requestFocus(); // Request focus for the ListView
                              _selectTag(tag);
                            },
                            child: Container(
                              padding: EdgeInsets.symmetric(
                                horizontal: 12.0,
                                vertical: 8.0,
                              ),
                              child: Text(tag),
                            ),
                          );
                        },
                      ),
                    ),
                  ),
              ],
            ),
          );
        }
      },
    );
  }
}
