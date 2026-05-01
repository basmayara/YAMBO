package com.example.Authentification.controller;

import com.example.Authentification.dto.RegisterDTO;
import com.example.Authentification.model.Player;
import com.example.Authentification.service.IPlayerService;
import com.example.Authentification.service.PlayerService;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.client.RestTemplate;
import org.springframework.http.*;

import java.util.Map;

@RestController
@RequestMapping("/api/player")
@CrossOrigin("*")
public class PlayerController {

    private final IPlayerService service;

    private final String P2_URL = "https://ungloved-overjoyed-gesture.ngrok-free.dev/api/auth/register";
    public PlayerController(PlayerService service) {
        this.service = service;
    }

    @PostMapping("/register")
    public Map<String, Object> register(@RequestBody RegisterDTO dto) {

        Player player = service.register(
                dto.getName(),
                dto.getEmail(),
                dto.getPassword()
        );

        try {
            RestTemplate restTemplate = new RestTemplate();
            HttpHeaders headers = new HttpHeaders();
            headers.setContentType(MediaType.APPLICATION_JSON);

            String body = "{\"email\":\"" + dto.getEmail() +
                    "\",\"password\":\"" + dto.getPassword() + "\"}";

            HttpEntity<String> entity = new HttpEntity<>(body, headers);
            restTemplate.postForObject(P2_URL, entity, String.class);

        } catch (Exception e) {
            System.out.println("P2 sync failed: " + e.getMessage());
        }

        return Map.of(
                "message", "Player registered successfully",
                "id", player.getId(),
                "name", player.getName(),
                "email", player.getEmail()
        );
    }

    @GetMapping("/me")
    public Map<String, Object> getMyPlayer(Authentication auth) {
        if (auth == null) {
            throw new RuntimeException("Unauthorized");
        }
        String email = auth.getName();
        Player player = service.createIfNotExists(email, "Unknown");
        return Map.of(
                "id", player.getId(),
                "email", player.getEmail(),
                "name", player.getName(),
                "level", player.getLevel(),
                "totalScore", player.getTotalScore()
        );
    }
}