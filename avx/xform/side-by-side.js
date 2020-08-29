function GoGet(book, chapter, maxChapter) {
    document.location = "/side-by-side/" + book + "/" + chapter.toString();
}

function BookChange() {
    var combo = document.getElementById('ComboBoxBook');
    var parts = combo.value.split(":")
    book = parts[0];
    var ch = parts[1];
    maxChapter = parseInt(ch);
    GoGet(book, 1, maxChapter);
}
