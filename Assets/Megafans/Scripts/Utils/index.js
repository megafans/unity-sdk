(() => {
    const container = document.querySelector('#container');

function openForm() {
    document.getElementById("addBookForm").style.display = "block";
    container.classList.add('is-blurred');
    addBook.classList.add('is-blurred');
  }
  
  function closeForm() {
    document.getElementById("addBookForm").style.display = "none";
    container.classList.remove('is-blurred');
    addBook.classList.remove('is-blurred');
  }

const addBook = document.getElementById('addBook');
addBook.addEventListener('click', openForm);

const saveButton = document.getElementById('save');
saveButton.addEventListener('click', addBookToLibrary);
saveButton.addEventListener('click', closeForm);


let myLibrary = [
    {
        "title": "Slaughterhouse Five",
        "author": "K. Vonnegut",
        "numPages": 240,
        "haveRead": true
    },
    {
        "title": "For Whom the Bell Tolls",
        "author": "E. Hemmingway",
        "numPages": 480,
        "haveRead": false
    },
    {
        "title": "The Great Gatsby",
        "author": "F. Scott Fitzgerald",
        "numPages": 208,
        "haveRead": true
    }
]

function Book(title, author, numPages, haveRead) { // Book object constructor
    this.title = title;
    this.author = author;
    this.numPages = numPages;
    this.haveRead = haveRead;
}

function addBookToLibrary() {
    const title = document.getElementsByName("title")[0].value
    const author = document.getElementsByName("author")[0].value
    const numPages = document.getElementsByName("numPages")[0].value
    const haveRead = false

    const book = new Book(title, author, numPages, haveRead);
    myLibrary.push(book);
    displayBooks();
}

function displayBooks() {
    container.innerText = '';
    for (let i = 0; i < myLibrary.length; i++) {
        createCard(i);
    }

    setCheckListeners();
    setDeleteListeners();
    console.log(myLibrary);
}

function createCard (i) {
    // Create HTML card element
    const card = document.createElement('div');
    card.classList.add('card');
    container.appendChild(card);

    // Add book information to card
    card.innerText = ('Title: ' + myLibrary[i].title + '\n' + 'Author: ' + myLibrary[i].author
                    + '\n' + 'Pages: ' + myLibrary[i].numPages  );
    
    // Create check box to mark if book has been read or not
    const readCheck = document.createElement('div');
    readCheck.classList.add('readCheck');
    card.appendChild(readCheck);
    readCheck.innerText = ("Have read: ")
    const readCheckmark = document.createElement("INPUT");
    readCheckmark.setAttribute("type", "checkbox");
    readCheckmark.setAttribute("id", "readCheckmark" + i);
    readCheck.appendChild(readCheckmark);
    if (myLibrary[i].haveRead == true) {
        document.getElementById('readCheckmark' + i).checked = true;
    }

    // Create delete button to remove book object from library array
    const deleteButton = document.createElement("INPUT");
    deleteButton.setAttribute("type", "button");
    deleteButton.setAttribute("id", "deleteButton" + i);
    deleteButton.setAttribute("value", "Delete Book");
    card.appendChild(deleteButton);
};

function setCheckListeners() {
    for (let i = 0; i < myLibrary.length; i++) {
        eval("let readCheckmark" + i + " = document.querySelector('readCheckmark" + i + "')");
        eval("readCheckmark" + i).addEventListener('change', function() {
            if (this.checked) {
                console.log(`Checkbox ${i} is checked.`);
                myLibrary[i].haveRead = true;
                console.log(myLibrary[i].haveRead)
            } else {
                console.log(`Checkbox ${i} is not checked`);
                myLibrary[i].haveRead = false;
                console.log(myLibrary[i].haveRead)
            }
        });
    }
}

function setDeleteListeners() {
    for (let i = 0; i < myLibrary.length; i++) {
        eval("let deleteButton" + i + " = document.querySelector('deleteButton" + i + "')");
        eval("deleteButton" + i).addEventListener('click', function() {
            // delete book object from myLibrary array
            console.log("Deleting " + myLibrary[i].title);
            myLibrary.splice(i, 1);
            displayBooks();
        })
    }
}
})();