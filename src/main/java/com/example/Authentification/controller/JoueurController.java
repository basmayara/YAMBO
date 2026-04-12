package com.example.Authentification.controller;

import com.example.Authentification.dto.RegisterDTO;
import com.example.Authentification.model.Joueur;
import com.example.Authentification.service.JoueurService;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import com.example.Authentification.security.JwtUtil;


import java.util.Map;

@RestController
@RequestMapping("/api/joueur")
@CrossOrigin(origins = "*")
public class JoueurController {

    private final JoueurService joueurService;

    public JoueurController(JoueurService joueurService) {
        this.joueurService = joueurService;
    }


    @PostMapping("/inscription")
    public ResponseEntity<?> inscrire(@Valid @RequestBody RegisterDTO dto) {
        try {
            Joueur joueur = joueurService.inscrire(dto);

            String token = JwtUtil.generateToken(joueur.getEmail());

            return ResponseEntity.status(201).body(Map.of(
                    "token", token,
                    "email", joueur.getEmail(),
                    "nom", joueur.getNom()
            ));

        } catch (RuntimeException e) {
            return ResponseEntity.badRequest().body(Map.of(
                    "erreur", e.getMessage()
            ));
        }
    }
    @GetMapping("/email/{email}")
    public ResponseEntity<?> getUserByEmail(@PathVariable String email) {
        Joueur joueur = joueurService.findByEmail(email);

        return ResponseEntity.ok(Map.of(
                "id", joueur.getId(),
                "email", joueur.getEmail(),
                "motDePasse", joueur.getMotDePasse()
        ));
    }

}
