$(function () {
  let startDate, endDate;
  let textValueObjs = {};
  let filtersObj = {};
  let qsRegex;
  let $container = $('#isotope2').isotope({
    itemSelector: '.tb-data-single',
    layoutMode: 'vertical',
    transitionDuration: 0
  });


  updateTotals();

  function updateFilters() {
    let filterValue = ''
    for (let prop in filtersObj) {
      if (filtersObj[prop] !== undefined) {
        filterValue += filtersObj[prop]
      }
    }
    // set filter for Isotope
    $container.isotope({
        filter: function () {
          let filterValue = '';
          let isMatched = true;
          let $this = $(this);
          let searchResult = qsRegex ? $this.text().match(qsRegex) : true;
          let dateResults = function () {
            if (startDate !== undefined && endDate !== undefined) {
              // _this_ is the item element. Get text of element's .number
              let dateVal = new Date($this.find('.transactionDate').text());
              // return true to show, false to hide
              return dateVal >= startDate && dateVal <= endDate;
            } else {
              return true;
            }
          };

          for (let prop in filtersObj) {
            // use function if it matches
            filterValue += filtersObj[prop]
            // test each filter
            if (filterValue) {
              isMatched = isMatched && $(this).is(filterValue);
            }
            // break if not matched
            if (!isMatched) {
              break;
            }
          }

          return searchResult && isMatched && dateResults($(this));
        }
      }
    )
    updateTotals();
  }

  function updateTotals() {
    if ($('#filterTotal').length) {
      let totalValue = 0;
      let itemElems = $container.isotope('getFilteredItemElements');
      $(itemElems).each(function () {
        let elId = $(this)[0];
        let $amount = $(elId).children('td.transactionAmount').text()
        let transactionAmount = Number($amount.replace(/[^0-9.-]+/g, ""));
        totalValue = parseFloat(transactionAmount) + totalValue;
      });

      $('#filterTotal').text(`£${totalValue.toFixed(2)}`)
    }
  }

  let $quicksearch = $('#quickSearch').keyup(debounce(function () {
    qsRegex = new RegExp($quicksearch.val(), 'gi');
    updateFilters();
  }));

  function updateTextValues() {
    $('.toggle-menu-parent').each(function () {
      const $this = $(this)
      const filterGroups = $this.data('filterGroup').split(', ')
      let textValue = []

      filterGroups.forEach(filterGroup => {
        if (textValueObjs[filterGroup] !== undefined) {
          textValue.push(textValueObjs[filterGroup])
        }
      })
      if (textValue.length) {
        textValue = textValue.join(', ')
        $this.children('.govuk-filter-group__container__subtitle').text(textValue)
      }
    })
  }

  function updateDateValues(formDataArray) {
    let formDateObject = {};

    $(formDataArray).each(function (index, obj) {
      formDateObject[obj.name] = obj.value;
    });

    let startDateString = `${formDateObject['start-date-month']}/${formDateObject['start-date-day']}/${formDateObject['start-date-year']}`;
    let endDateString = `${formDateObject['end-date-month']}/${formDateObject['end-date-day']}/${formDateObject['end-date-year']}`;

    startDate = convertDate(startDateString);
    endDate = convertDate(endDateString);

    return `from ${startDateString} to ${endDateString}`
  }

  $('.filter-button').on('click', function (e) {
    e.preventDefault();
    const $this = $(this);
    let newSubtitle = $this.text();
    const filterGroup = $this.data('filter-group');

    newSubtitle = newSubtitle.trim();

    filtersObj[filterGroup] = $this.data('filter');
    textValueObjs[filterGroup] = newSubtitle;

    updateFilters();
    updateTextValues()
  });

  $('.filter-form-select').on('submit', function (e) {
    e.preventDefault();
    const $this = $(this);
    const filterGroup = $this.data('filter-group');
    let filtersArray = [];
    let textArray = [];

    $this.find('input:checked').each(function () {
      const $this = $(this);
      filtersArray.push($this.data('filter'))
      textArray.push($this.parents('label').text().trim())
    });

    filtersObj[filterGroup] = filtersArray;
    textValueObjs[filterGroup] = textArray;

    updateFilters();
    updateTextValues();
    return false
  });

  function convertDate(dateString) {
    return new Date(dateString);
  }

  $('#byDateRange').on('submit', function (e) {
    e.preventDefault();
    const $this = $(this);
    const filterGroup = $this.data('filter-group');

    const formDataArray = $this.serializeArray();
    let formDateObject = {};

    $(formDataArray).each(function (index, obj) {
      formDateObject[obj.name] = obj.value;
    });

    let startDateString = `${formDateObject['start-date-month']}/${formDateObject['start-date-day']}/${formDateObject['start-date-year']}`;
    let endDateString = `${formDateObject['end-date-month']}/${formDateObject['end-date-day']}/${formDateObject['end-date-year']}`;

    startDate = convertDate(startDateString);
    endDate = convertDate(endDateString);

    let textArray = [];
    textArray.push(`from ${startDateString} to ${endDateString}`);

    textValueObjs['transactionDate'] = textArray;
    updateFilters();
    updateTextValues();
    return false;
  });

  function debounce(fn, threshold) {
    var timeout;
    threshold = threshold || 100;
    return function debounced() {
      clearTimeout(timeout);
      var args = arguments;
      var _this = this;

      function delayed() {
        fn.apply(_this, args);
      }

      timeout = setTimeout(delayed, threshold);
    };
  }

})


