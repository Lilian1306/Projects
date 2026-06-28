import express from 'express';
import { adminLogin, registerUser } from '../controllers/userController.js';

const userRouter = express.Router();

userRouter.post('/register', registerUser)
userRouter.post('/admin', adminLogin)

export default userRouter;