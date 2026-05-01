package com.example.Authentification.service;

import com.example.Authentification.model.Player;

public interface IPlayerService {
    Player register(String name, String email, String password);
    Player findByEmail(String email);
    Player createIfNotExists(String email,String name);
}