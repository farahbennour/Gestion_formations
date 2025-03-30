// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Gestion de la confirmation du type d'utilisateur



//SignUp
document.getElementById('confirmUserType').addEventListener('click', function () {
    var userType = document.getElementById('UserType').value;
    var modal = document.getElementById('userTypeModal');
    var form = document.getElementById('signupForm');
    var specialiteField = document.getElementById('specialiteField');
    var experienceField = document.getElementById('experienceField');
    var dateEmbaucheField = document.getElementById('dateEmbaucheField');

    if (userType) {
        modal.style.display = 'none';
        form.style.display = 'block';

        if (userType === 'Formateur') {
            specialiteField.style.display = 'block';
            experienceField.style.display = 'block';
            dateEmbaucheField.style.display = 'block';
        } else {
            specialiteField.style.display = 'none';
            experienceField.style.display = 'none';
            dateEmbaucheField.style.display = 'none';
        }
        document.getElementById('Role').value = userType;
    } else {
        alert('Veuillez choisir un type d\'utilisateur.');
    }
    if (userType === 'Formateur') {
        specialiteField.style.display = 'block';
        experienceField.style.display = 'block';
        dateEmbaucheField.style.display = 'block';
        // Ajoutez le required ici si nécessaire
        document.getElementById('Specialite').required = true;
        document.getElementById('Experience').required = true;
        document.getElementById('DateEmbauche').required = true;
    } else {
        specialiteField.style.display = 'none';
        experienceField.style.display = 'none';
        dateEmbaucheField.style.display = 'none';
        // Retirer le required des champs cachés
        document.getElementById('Specialite').required = false;
        document.getElementById('Experience').required = false;
        document.getElementById('DateEmbauche').required = false;
    }

});

// Validation avant soumission du formulaire
document.getElementById('signupForm').addEventListener('submit', function (e) {
    var userType = document.getElementById('UserType').value;
    if (!userType) {
        e.preventDefault(); // Empêche la soumission du formulaire
        alert('Veuillez choisir un type d\'utilisateur.');
    }
});







