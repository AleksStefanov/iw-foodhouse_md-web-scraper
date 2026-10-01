# iw-foodhouse_md-web-scraper

## Web scraper designed to export restaurant and article data from foodhouse.md.

### Output :
Two files:
- One containing all restaurants data in table format (id, name, address)
- One containing all articles data in table format (id, restaurant's id, name, price)


### Assumptions :
- All restaurants are loaded in foodhouse.md/en/restaurants without pagination or lazy loading;
- All menu items are loaded in foodhouse.md/en/restaurant/{item_id} without pagination or lazy loading;
- Parallel traffic to the domain won't be blocked by a limiter;
- In case a restaurants gets deleted after its data is fetched but before articles are fetched, the restourant will stay in the final output;

### Implementation Overview : 
Step 1: Loading /restaurants page 
Step 2: Gathering all restourants name and id data from the Drupal Settings
Step 3: Parallelly loading each restourants page at /restaurant/{id}
Step 4: Gethering restourant's addresses(DOM) and menu items(Drupal Settings)
Step 5: pushing each restourant and its items data to a channel, streaming it directly into I/O CSV file
Step 6: Exit








