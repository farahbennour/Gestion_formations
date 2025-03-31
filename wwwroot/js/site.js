//// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
//// for details on configuring this project to bundle and minify static web assets.

//// Write your JavaScript code.

//// Gestion de la confirmation du type d'utilisateur



////SignUp
//document.getElementById('confirmUserType').addEventListener('click', function () {
//    var userType = document.getElementById('UserType').value;
//    var modal = document.getElementById('userTypeModal');
//    var form = document.getElementById('signupForm');
//    var specialiteField = document.getElementById('specialiteField');
//    var experienceField = document.getElementById('experienceField');
//    var dateEmbaucheField = document.getElementById('dateEmbaucheField');

//    if (userType) {
//        modal.style.display = 'none';
//        form.style.display = 'block';

//        if (userType === 'Formateur') {
//            specialiteField.style.display = 'block';
//            experienceField.style.display = 'block';
//            dateEmbaucheField.style.display = 'block';
//        } else {
//            specialiteField.style.display = 'none';
//            experienceField.style.display = 'none';
//            dateEmbaucheField.style.display = 'none';
//        }
//        document.getElementById('Role').value = userType;
//    } else {
//        alert('Veuillez choisir un type d\'utilisateur.');
//    }
//    if (userType === 'Formateur') {
//        specialiteField.style.display = 'block';
//        experienceField.style.display = 'block';
//        dateEmbaucheField.style.display = 'block';
//        // Ajoutez le required ici si nécessaire
//        document.getElementById('Specialite').required = true;
//        document.getElementById('Experience').required = true;
//        document.getElementById('DateEmbauche').required = true;
//    } else {
//        specialiteField.style.display = 'none';
//        experienceField.style.display = 'none';
//        dateEmbaucheField.style.display = 'none';
//        // Retirer le required des champs cachés
//        document.getElementById('Specialite').required = false;
//        document.getElementById('Experience').required = false;
//        document.getElementById('DateEmbauche').required = false;
//    }

//});

//// Validation avant soumission du formulaire
//document.getElementById('signupForm').addEventListener('submit', function (e) {
//    var userType = document.getElementById('UserType').value;
//    if (!userType) {
//        e.preventDefault(); // Empêche la soumission du formulaire
//        alert('Veuillez choisir un type d\'utilisateur.');
//    }
//});



//    function validateForm(event) {
//    let password = document.getElementById("PasswordHash").value;
//    let newPassword = document.getElementById("newPassword").value;
//    let telephone = document.getElementById("Telephone").value;

//    let passwordError = document.getElementById("passwordError");
//    let newPasswordError = document.getElementById("newPasswordError");
//    let telephoneError = document.getElementById("telephoneError");

//    let passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
//    let telephoneRegex = /^\d{8}$/;

//    let isValid = true;

//    // Réinitialiser les messages d'erreur
//    passwordError.innerText = "";
//    newPasswordError.innerText = "";
//    telephoneError.innerText = "";

//    // Vérification du mot de passe
//    if (!passwordRegex.test(password)) {
//        passwordError.innerText = "Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.";
//    isValid = false;
//        }

//        // Vérification du nouveau mot de passe (s'il est rempli)
//        if (newPassword.length > 0 && !passwordRegex.test(newPassword)) {
//        newPasswordError.innerText = "Le nouveau mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.";
//    isValid = false;
//        }

//    // Vérification du téléphone
//    if (!telephoneRegex.test(telephone)) {
//        telephoneError.innerText = "Le numéro de téléphone doit contenir exactement 8 chiffres.";
//    isValid = false;
//    }

//    // Si une erreur existe, empêcher l'envoi
//    if (!isValid) {
//        event.preventDefault(); // Bloque la soumission
//        }

//    return isValid;
//}

//document.querySelector('form').addEventListener('submit', function (event) {
//    var password = document.getElementById('newPassword').value;
//    var passwordPattern = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

//    if (!passwordPattern.test(password)) {
//        alert('Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.');
//        event.preventDefault(); // Empêcher la soumission du formulaire
//    }
//});




//    //function togglePasswordVisibility(inputId) {
//    //    const passwordInput = document.getElementById(inputId);
//    //const eyeIcon = passwordInput.nextElementSibling.querySelector('i');

//    //if (passwordInput.type === 'password') {
//    //    passwordInput.type = 'text';
//    //eyeIcon.classList.remove('bi-eye-slash');
//    //eyeIcon.classList.add('bi-eye');
//    //    } else {
//    //    passwordInput.type = 'password';
//    //eyeIcon.classList.remove('bi-eye');
//    //eyeIcon.classList.add('bi-eye-slash');
//    //    }
//    //}


//     //document.getElementById("profileForm").addEventListener("submit", function(event) {
//     //       let password = document.getElementById("NewPassword").value;
//     //       let passwordError = document.getElementById("newPasswordError");

//     //       // Regex pour valider le mot de passe : au moins 8 caractères, lettres, chiffres et symboles
//     //       let passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

//     //       let isValid = true;

//     //       // Réinitialiser le message d'erreur
//     //       passwordError.innerText = "";

//     //       // Vérification du mot de passe
//     //       if (!passwordRegex.test(password)) {
//     //           passwordError.innerText = "Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.";
//     //           isValid = false;
//     //       }

//     //       // Si le mot de passe est invalide, empêcher l'envoi du formulaire
//     //       if (!isValid) {
//     //           event.preventDefault(); // Bloquer la soumission
//     //       }

//     //       return isValid; // Retourne false pour empêcher l'envoi si invalid
//     //   });






