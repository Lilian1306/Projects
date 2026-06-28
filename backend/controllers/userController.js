import validator from 'validator'
import bcrypt from 'bcryptjs'
import jwt from 'jsonwebtoken'
import userModel from '../models/userModel.js'

const createToken = (id) => {
    return jwt.sign({id}, process.env.JWT_SECRET)
}

// Route for user register
const registerUser = async (req, res) => {
    try{
        const {name, email, password} = req.body;
        
        // checking user already exist or not
        const exist = await userModel.findOne({email});
        if(exist){
            return res.json({success:false, message: 'User alreay exist'})
        }

        if(!validator.isEmail(email)){
            return res.json({success:false, message:'Please enter a valid email'})
        }
        if(password.lenght < 8){
            return res.json({success:false, message:'Please enter a strong password'})
        }
        // hashing user password
        const salt = await bcrypt.genSalt(10)
        const hashedPassword = await bcrypt.hash(password, salt)

        const newUser = new userModel({
            name,
            email, 
            password:hashedPassword
        })

        const user = await newUser.save()
        const token = createToken(user._id)
        res.json({success:true, token })
    }catch(error){
        console.log(error)
        res.json({success:false, message:error.message})
    }
}

//Adming login
const adminLogin = async (req, res) => {
    try {
        const {email, password} = req.body
        
        if(email === process.env.ADMIN_EMAIL && password === process.env.ADMIN_PASSWORD){
           const token =  jwt.sign(email+password, process.env.JWT_SECRET)
           res.json({success:true,token})
        }else{
            res.json({success:false,message: 'Invalid credentials'})
        }

        
    }catch(error){
         console.log(error)
         res.json({succes: false, message: error.message})
    }

}

export {adminLogin, registerUser}