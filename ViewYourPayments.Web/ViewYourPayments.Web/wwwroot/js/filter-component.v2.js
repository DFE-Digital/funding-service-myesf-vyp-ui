$(function () {
  let startDate, endDate;
  let $multiSelect = $('.multipleSelect');
  let filtersObj = {};
  let $filterTotal = $('#filterTotal');
  let $container = $('#transaction-table-v2').isotope({
    itemSelector: '.tb-data-single',
    layoutMode: 'vertical',
    transitionDuration: 0
  });
  $multiSelect.fastselect();
  updateTotals();

  function updateFilters() {
    let filterValue = '';
    function escapeRegExp(string) {
      return string.replace(/[*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
    }
    // set filter for Isotope

    $container.isotope({
        filter: function () {
          let filterValue = '';
          let isMatched = true;
          let $this = $(this);
          let dateResults = function () {
            if (startDate !== undefined && endDate !== undefined) {
              // _this_ is the item element. Get text of element's .number
              let dateVal = new Date($this.data('transactionDate'));
              console.log($this.data('transactionDate'))
              // return true to show, false to hide
              return dateVal >= startDate && dateVal <= endDate;
            } else {
              return true;
            }
          };
          for (let prop in filtersObj) {
            // use function if it matches
            filterValue += filtersObj[prop];
            // test each filter
            if (filterValue) {
              let searchParams = filterValue.split(",");
              let stringFound = false;
              let i = 0;
              while (searchParams[i] && stringFound === false) {
                let searchString =  escapeRegExp(searchParams[i]);
                let globalRegex = new RegExp(searchString, 'gi');
                let str = $this.find('.description').text();
                stringFound =  globalRegex.test(str);
                i++;
              }              
              escapedFilterValue = escapeRegExp(filterValue)

              let classOrSearch =  stringFound || $(this).is(escapedFilterValue);
              isMatched = isMatched && classOrSearch;
            }
            // break if not matched
            if (!isMatched) {
              break;
            }
          }
          return isMatched && dateResults($(this));
        }
      }
    );
    updateTotals();
  }
  function updateTotals(){
    if($filterTotal.length){
      let totalValue = 0;
      let itemElems = $container.isotope('getFilteredItemElements');
      $(itemElems).each(function(){
        let elId = $(this)[0];
        let $amount = $(elId).children('td.transactionAmount').text()
        let transactionAmount = Number($amount.replace(/[^0-9.-]+/g,""));
        totalValue = parseFloat(transactionAmount) + totalValue;
      });
      // Create our number formatter.
      var formatter = new Intl.NumberFormat('en-UK', {
        style: 'currency',
        currency: 'GBP',
      });

      totalValue = formatter.format(totalValue);
      $filterTotal.text(`${totalValue}`)
    }
  }
  function convertDate(dateString) {
    return new Date(dateString);
  }
  $('#filters').on('submit', function(e){
    e.preventDefault();
    const $this = $(this);
    let formData = $this.serializeArray();
    let formDateObject = {};

    $(formData).each(function (index, obj) {
      formDateObject[obj.name] = obj.value;
    });

    let startDateString = `${formDateObject['start-date-month']}/${formDateObject['start-date-day']}/${formDateObject['start-date-year']}`;
    let endDateString = `${formDateObject['end-date-month']}/${formDateObject['end-date-day']}/${formDateObject['end-date-year']}`;

    startDate = convertDate(startDateString);
    endDate = convertDate(endDateString);

    $multiSelect.each(function () {
      const filterGroup = $(this).data('filter-group');
      const selectedFilters = $(this).data('fastselect').optionsCollection.selectedValues;
      let filtersArray = [];
      for (const filter in selectedFilters) {
        filtersArray.push(filter);
      }
      filtersObj[filterGroup] = filtersArray;
    });
    updateFilters();
    return false;
  }).on('reset', function (e) {
    $('.fstChoiceRemove').each(function(){
      $(this).trigger('click');
    });
    $(this).submit();
  })

});


